using StockFlow360.Application.Interfaces;
using StockFlow360.Domain.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO;

namespace StockFlow360.Infrastructure.Services
{
    public class InvoiceService : IInvoiceService
    {
        public async Task<byte[]> GenerateSaleInvoiceAsync(Sale sale)
        {
            var invoiceNumber = $"INV-{sale.Id:D6}";

            var items = sale.Items.Select(i => new InvoiceItem
            {
                Name = i.Product?.Name ?? $"Product #{i.ProductId}",
                Quantity = i.Quantity,
                UnitPrice = i.SellingPrice,
                TaxRate = i.TaxRate,
                TaxAmount = i.TaxAmount,
                Total = i.SellingPrice * i.Quantity,
                TotalWithTax = i.TotalWithTax
            }).ToList();

            return await Task.FromResult(GenerateInvoice(
                "SALE INVOICE",
                invoiceNumber,
                sale.SaleDate,
                items,
                items.Sum(i => i.Total),
                items.Sum(i => i.TotalWithTax)
            ));
        }

        public async Task<byte[]> GeneratePurchaseInvoiceAsync(Purchase purchase)
        {
            var invoiceNumber = $"PINV-{purchase.Id:D6}";

            var items = purchase.Items.Select(i => new InvoiceItem
            {
                Name = i.Product?.Name ?? $"Product #{i.ProductId}",
                Quantity = i.Quantity,
                UnitPrice = i.CostPrice,
                Total = i.CostPrice * i.Quantity,
                TotalWithTax = i.CostPrice * i.Quantity 
            }).ToList();

            return await Task.FromResult(GenerateInvoice(
                "PURCHASE INVOICE",
                invoiceNumber,
                purchase.PurchaseDate,
                items,
                items.Sum(i => i.Total),
                items.Sum(i => i.TotalWithTax)
            ));
        }


        private byte[] GenerateInvoice(string title, string invoiceNumber, DateTime date, IEnumerable<InvoiceItem> items, decimal subtotal, decimal grandTotal)
        {
            using var stream = new MemoryStream();

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("StockFlow360").FontSize(24).Bold();
                            col.Item().Text(title).FontSize(16);
                            col.Item().Text($"Invoice #: {invoiceNumber}").FontSize(12);
                            col.Item().Text($"Date: {date:yyyy-MM-dd HH:mm}").FontSize(12);
                        });

                        row.ConstantItem(100).Image("data/StockFlow360_logo.png").FitWidth();
                    });

                    page.Content().PaddingVertical(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();     // Item Name
                            columns.ConstantColumn(40);   // Quantity
                            columns.ConstantColumn(70);   // Price
                            columns.ConstantColumn(70);   // Tax
                            columns.ConstantColumn(80);   // Total
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Item").Bold();
                            header.Cell().Text("Qty").Bold();
                            header.Cell().Text("Price").Bold();
                            header.Cell().Text("Tax").Bold();
                            header.Cell().Text("Total").Bold();
                        });

                        foreach (var item in items)
                        {
                            table.Cell().Text(item.Name);
                            table.Cell().Text(item.Quantity.ToString());
                            table.Cell().Text(item.UnitPrice.ToString("C"));
                            table.Cell().Text(item.TaxAmount.ToString("C"));
                            table.Cell().Text(item.TotalWithTax.ToString("C"));
                        }
                    });

                    page.Footer().AlignRight().Column(col =>
                    {
                        col.Item().Text($"Subtotal: {subtotal.ToString("C")}");
                        col.Item().Text($"Grand Total(with tax): {grandTotal.ToString("C")}").Bold().FontSize(14);
                    });
                });
            }).GeneratePdf(stream);

            return stream.ToArray();
        }

        private class InvoiceItem
        {
            public string Name { get; set; } = string.Empty;
            public int Quantity { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal TaxRate { get; set; }
            public decimal TaxAmount { get; set; }
            public decimal Total { get; set; }
            public decimal TotalWithTax { get; set; }
        }
    }
}
