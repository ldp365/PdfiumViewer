//------------------------------版权所有：XXXXXX------------------------------//
// 文件名:	***.cs
// 说明:	无
// 作成者:	李大鹏|WeChat:ldp365
// 作成日:	2016/09/28
// 作成说明:无
// 需求变更记录↓
// 
// 修改者:	无
// 修改日:	无
// 修改说明:无
//----------------此技术信息为本公司机密信息，未经本公司书面同意禁止向第三方披露----------------//
namespace PdfiumViewer
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Drawing.Imaging;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using System.Text;

	public class API
	{
		/// <summary>
		/// PDF转JPG 每页一个图片
		/// </summary>
		/// <param name="strPDFPath"></param>
		/// <param name="strJPGPath"></param>
		/// <param name="bAutoSize"></param>
		/// <param name="iDPI">当分辨率是72像素/英寸时，A4纸像素长宽分别是842×595；
		/// 当分辨率是120像素/英寸时，A4纸像素长宽分别是2105×1487；
		///当分辨率是150像素/英寸时，A4纸像素长宽分别是1754×1240；
		///当分辨率是300像素/英寸时，A4纸像素长宽分别是3508×2479.</param>
		/// <returns></returns>
		public static List<string> PDF2JPG(string strPDFPath, string strJPGPath, bool bAutoZoom = true, int iDPI = 150)
		{
			List<string> lstResult = new List<string>();
			try
			{
				if (!File.Exists(strPDFPath) || strJPGPath.LastIndexOf(".") <= 0)
				{
					return lstResult;
				}
				if (!z7())
				{
					return lstResult;
				}
				var pdf = PdfDocument.Load(strPDFPath);
				var pdfpage = pdf.PageCount;
				var pagesizes = pdf.PageSizes;
				string strPath = strJPGPath.Substring(0, strJPGPath.LastIndexOf("."));
				string strExe = strJPGPath.Substring(strJPGPath.LastIndexOf("."));
				string strTmpPaht;
				int iWidth = 0;
				if (bAutoZoom)
				{
					switch (iDPI)
					{
						case 300:
							iWidth = 2479;
							break;
						case 150:
							iWidth = 1240;
							break;
						case 120:
							iWidth = 1487;
							break;
						case 72:
							iWidth = 595;
							break;
					}
				}

				for (int i = 0; i < pdfpage; i++)
				{
					Size size = new Size();
					float fZoom = 1.0F;// 1.5F
					if (iWidth > 0)
					{
						fZoom = iWidth / pagesizes[(i)].Width;
						if (fZoom > 1.5F)
						{
							int index = fZoom.ToString().IndexOf(".");
							if (index > 0)
							{
								int iZS = Convert.ToInt32(fZoom.ToString().Substring(0, index));
								decimal decXS = Convert.ToDecimal("0" + fZoom.ToString().Substring(index));
								if (decXS >= 0.5M)
								{
									fZoom = iZS + 0.5F;
								}
								else
								{
									fZoom = iZS;
								}
							}
						}
						else
						{
							fZoom = 1.0F;
						}
					}
					size.Height = (int)(pagesizes[(i)].Height * fZoom);
					size.Width = (int)(pagesizes[(i)].Width * fZoom);
					strTmpPaht = strJPGPath;
					if (pdfpage > 1)
					{
						strTmpPaht = strPath + $"_{i}" + strExe;
					}
					RenderPage(strPDFPath, i, size, strTmpPaht, iDPI);
					lstResult.Add(strTmpPaht);
				}
			}
			catch (Exception ex)
			{
			}
			return lstResult;
		}

		private static bool z7()
		{
			if (File.Exists(System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, @"pdfium.7z")))
			{
				try
				{
					Assembly loader = Assembly.LoadFrom("7zCSharp.dll");
					Type typeSevenZipExtractor = loader.GetType("SevenZip.SevenZipBase");
					object obj = typeSevenZipExtractor.InvokeMember("SetLibraryPath",
								System.Reflection.BindingFlags.InvokeMethod | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public, null, null,
								new object[] { System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, @"7zC.dll") });

					Type ty = loader.GetType("SevenZip.SevenZipExtractor");
					object magicConstructor = Activator.CreateInstance(ty, new object[] { System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, @"pdfium.7z") });//获取不带参数的构造函数
					MethodInfo funSend = magicConstructor.GetType().GetMethod("ExtractArchive");
					funSend.Invoke(magicConstructor, new object[] { System.Windows.Forms.Application.StartupPath });
					File.Delete(System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, @"pdfium.7z"));
				}
				catch (Exception)
				{
					System.Windows.Forms.MessageBox.Show("PDF内部库解压失败！", "PDF使用提示", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
					return false;
				}
			}

			if (File.Exists(System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, @"pdfium.dll")))
			{
				return true;
			}
			System.Windows.Forms.MessageBox.Show("PDF内部库：pdfium.dll没有找到！", "PDF使用提示", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
			return false;
		}

		/// <summary>
		/// 将PDF转换为图片
		/// </summary>
		/// <param name="pdfPath">pdf文件位置</param>
		/// <param name="pageNumber">pdf文件张数</param>
		/// <param name="size">pdf文件尺寸</param>
		/// <param name="outputPath">输出图片位置与名称</param>
		public static void RenderPage(string pdfPath, int pageNumber, Size size, string outputPath, int iDPI = 300, ImageFormat imageFormat = null)
		{
			using (var document = PdfiumViewer.PdfDocument.Load(pdfPath))
			using (var stream = new FileStream(outputPath, FileMode.Create))
			using (var image = GetPageImage(pageNumber, size, document, iDPI))
			{
				image.Save(stream, imageFormat ?? ImageFormat.Jpeg);
			}
		}
		private static Image GetPageImage(int pageNumber, Size size, PdfDocument document, int iDPI)
		{
			return document.Render(pageNumber, size.Width, size.Height, iDPI, iDPI, PdfRenderFlags.Annotations);
		}
	}
}
