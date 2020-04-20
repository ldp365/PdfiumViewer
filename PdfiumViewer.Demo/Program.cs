using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace PdfiumViewer.Demo
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
			PdfiumViewer.API.PDF2JPG(Path.Combine(Application.StartupPath, @"平安.pdf"), Path.Combine(Application.StartupPath, @"平安A.jpg"));
		//	var pdf = PdfDocument.Load(Path.Combine(Application.StartupPath, @"平安.pdf"));
			//var pdfpage = pdf.PageCount;
			//var pagesizes = pdf.PageSizes;

			//PdfToPicture p2p = new PdfToPicture();
			//for (int i = 1; i <= pdfpage; i++)
			//{
			//	Size size = new Size();
			//	size.Height = (int)(pagesizes[(i - 1)].Height * 1.5F);
			//	size.Width = (int)(pagesizes[(i - 1)].Width * 1.5F);
			//	p2p.RenderPage(Path.Combine(Application.StartupPath, @"平安.pdf"), i, size, Path.Combine(Application.StartupPath, $"平安AA{i}.jpg"));
			//}
			Console.WriteLine("Success");
			Console.Read();
			System.Windows.Forms.MessageBox.Show("aaa");
			//Application.EnableVisualStyles();
   //         Application.SetCompatibleTextRenderingDefault(false);
   //         Application.Run(new MainForm());
        }

		class PdfToPicture
		{
			/// <summary>
			/// 将PDF转换为图片
			/// </summary>
			/// <param name="pdfPath">pdf文件位置</param>
			/// <param name="pageNumber">pdf文件张数</param>
			/// <param name="size">pdf文件尺寸</param>
			/// <param name="outputPath">输出图片位置与名称</param>
			public void RenderPage(string pdfPath, int pageNumber, System.Drawing.Size size, string outputPath, int dpi = 300)
			{
				using (var document = PdfiumViewer.PdfDocument.Load(pdfPath))
				using (var stream = new FileStream(outputPath, FileMode.Create))
				using (var image = GetPageImage(pageNumber, size, document, dpi))
				{
					image.Save(stream, ImageFormat.Jpeg);
				}
			}
			private static Image GetPageImage(int pageNumber, Size size, PdfiumViewer.PdfDocument document, int dpi)
			{
				return document.Render(pageNumber - 1, size.Width, size.Height, dpi, dpi, PdfRenderFlags.Annotations);
			}
		}
	}
}
