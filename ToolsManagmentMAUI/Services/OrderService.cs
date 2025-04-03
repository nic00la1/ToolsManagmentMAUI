using System.Collections.ObjectModel;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System.Diagnostics;
using ToolsManagmentMAUI.Models;

namespace ToolsManagmentMAUI.Services
{
    public class OrderService
    {
        private readonly ShoppingCartService _shoppingCartService;

        public OrderService(ShoppingCartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
        }

        public void ProcessOrder(ObservableCollection<ShoppingCartItem> cartItems)
        {
            // Update available tool quantities
            foreach (var item in cartItems)
            {
                var availableTool = _shoppingCartService.GetAvailableTool(item.Tool.Id);
                if (availableTool != null)
                {
                    availableTool.Quantity -= item.Quantity;
                }
            }

            // Generate invoice
            GenerateInvoice(cartItems);
        }

        private void GenerateInvoice(ObservableCollection<ShoppingCartItem> cartItems)
        {
            using (var document = new PdfDocument())
            {
                var page = document.AddPage();
                var gfx = XGraphics.FromPdfPage(page);
                var font = new XFont("Verdana", 20, XFontStyle.Bold);

                gfx.DrawString("Faktura", font, XBrushes.Black,
                    new XRect(0, 0, page.Width, page.Height),
                    XStringFormats.TopCenter);

                font = new XFont("Verdana", 12, XFontStyle.Regular);
                int yPoint = 40;

                // Column headers
                gfx.DrawString("Nazwa", font, XBrushes.Black,
                    new XRect(40, yPoint, page.Width, page.Height),
                    XStringFormats.TopLeft);
                gfx.DrawString("Iloœæ", font, XBrushes.Black,
                    new XRect(200, yPoint, page.Width, page.Height),
                    XStringFormats.TopLeft);
                gfx.DrawString("Cena", font, XBrushes.Black,
                    new XRect(300, yPoint, page.Width, page.Height),
                    XStringFormats.TopLeft);
                gfx.DrawString("£¹cznie", font, XBrushes.Black,
                    new XRect(400, yPoint, page.Width, page.Height),
                    XStringFormats.TopLeft);
                yPoint += 20;

                // Items
                foreach (var item in cartItems)
                {
                    gfx.DrawString(item.Tool.Name, font, XBrushes.Black,
                        new XRect(40, yPoint, page.Width, page.Height),
                        XStringFormats.TopLeft);
                    gfx.DrawString(item.Quantity.ToString(), font, XBrushes.Black,
                        new XRect(200, yPoint, page.Width, page.Height),
                        XStringFormats.TopLeft);
                    gfx.DrawString($"{item.Tool.Price:C}", font, XBrushes.Black,
                        new XRect(300, yPoint, page.Width, page.Height),
                        XStringFormats.TopLeft);
                    gfx.DrawString($"{item.TotalPrice:C}", font, XBrushes.Black,
                        new XRect(400, yPoint, page.Width, page.Height),
                        XStringFormats.TopLeft);
                    yPoint += 20;
                }

                // Total
                gfx.DrawString($"Razem: {cartItems.Sum(item => item.TotalPrice):C}", font, XBrushes.Black,
                    new XRect(40, yPoint, page.Width, page.Height),
                    XStringFormats.TopLeft);

                string filename = "Faktura.pdf";
                document.Save(filename);
                Process.Start(new ProcessStartInfo(filename) { UseShellExecute = true });
            }
        }
    }
}