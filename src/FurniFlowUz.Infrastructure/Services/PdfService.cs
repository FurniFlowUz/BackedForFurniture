using FurniFlowUz.Domain.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace FurniFlowUz.Infrastructure.Services;

public interface IPdfService
{
    /// <summary>
    /// Generates a PDF for a contract
    /// </summary>
    Task<byte[]> GenerateContractPdfAsync(Contract contract);

    /// <summary>
    /// Generates a PDF for an order
    /// </summary>
    Task<byte[]> GenerateOrderPdfAsync(Order order);

    /// <summary>
    /// Generates a PDF for a technical specification
    /// </summary>
    Task<byte[]> GenerateTechnicalSpecPdfAsync(TechnicalSpecification technicalSpec);
}

public class PdfService : IPdfService
{
    private const string CompanyName = "FurniFlow Uz";
    private const string CompanyAddress = "Tashkent, Uzbekistan";
    private const string CompanyPhone = "+998 (71) 123-45-67";
    private const string CompanyEmail = "info@furniflow.uz";

    public PdfService()
    {
        // Configure QuestPDF license
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public Task<byte[]> GenerateContractPdfAsync(Contract contract)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header().Element(ComposeHeader);

                page.Content().PaddingVertical(1, Unit.Centimetre).Column(column =>
                {
                    column.Spacing(10);

                    // Title
                    column.Item().AlignCenter().Text("CONTRACT").Bold().FontSize(18);

                    // Contract Information
                    column.Item().BorderBottom(1).PaddingBottom(5).Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text($"Contract Number: {contract.ContractNumber}").Bold();
                            col.Item().Text($"Date: {contract.SignedDate?.ToString("dd MMMM yyyy") ?? "Not signed"}");
                            col.Item().Text($"Status: {contract.Status}").Bold();
                        });

                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text($"Production Duration: {contract.ProductionDurationDays} days");
                            col.Item().Text($"Payment Status: {contract.PaymentStatus}");
                        });
                    });

                    // Customer Information
                    column.Item().PaddingTop(10).Text("Customer Information").Bold().FontSize(14);
                    column.Item().BorderBottom(1).PaddingBottom(5).Column(col =>
                    {
                        col.Item().Text($"Name: {contract.Customer?.FullName ?? "N/A"}");
                        col.Item().Text($"Phone: {contract.Customer?.PhoneNumber ?? "N/A"}");
                        col.Item().Text($"Email: {contract.Customer?.Email ?? "N/A"}");
                        if (!string.IsNullOrWhiteSpace(contract.Customer?.Address))
                        {
                            col.Item().Text($"Address: {contract.Customer.Address}");
                        }
                    });

                    // Financial Details
                    column.Item().PaddingTop(10).Text("Financial Details").Bold().FontSize(14);
                    column.Item().BorderBottom(1).PaddingBottom(5).Column(col =>
                    {
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Text("Total Amount:");
                            row.RelativeItem().AlignRight().Text($"{contract.TotalAmount:N2} UZS").Bold();
                        });
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Text("Advance Payment:");
                            row.RelativeItem().AlignRight().Text($"{contract.AdvancePaymentAmount:N2} UZS");
                        });
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Text("Remaining Amount:");
                            row.RelativeItem().AlignRight().Text($"{contract.RemainingAmount:N2} UZS").Bold();
                        });
                    });

                    // Contract Terms
                    if (!string.IsNullOrWhiteSpace(contract.DeliveryTerms))
                    {
                        column.Item().PaddingTop(10).Text("Delivery Terms").Bold().FontSize(14);
                        column.Item().BorderBottom(1).PaddingBottom(5).Text(contract.DeliveryTerms);
                    }

                    if (!string.IsNullOrWhiteSpace(contract.PenaltyTerms))
                    {
                        column.Item().PaddingTop(10).Text("Penalty Terms").Bold().FontSize(14);
                        column.Item().BorderBottom(1).PaddingBottom(5).Text(contract.PenaltyTerms);
                    }

                    if (!string.IsNullOrWhiteSpace(contract.AdditionalNotes))
                    {
                        column.Item().PaddingTop(10).Text("Additional Notes").Bold().FontSize(14);
                        column.Item().BorderBottom(1).PaddingBottom(5).Text(contract.AdditionalNotes);
                    }

                    // Signatures
                    column.Item().PaddingTop(30).Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("Company Representative").FontSize(10);
                            col.Item().PaddingTop(20).BorderTop(1).Width(150).Text("Signature & Stamp");
                        });

                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("Customer").FontSize(10);
                            col.Item().PaddingTop(20).BorderTop(1).Width(150).Text("Signature");
                        });
                    });
                });

                page.Footer().Element(ComposeFooter);
            });
        });

        var pdfBytes = document.GeneratePdf();
        return Task.FromResult(pdfBytes);
    }

    public Task<byte[]> GenerateOrderPdfAsync(Order order)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header().Element(ComposeHeader);

                page.Content().PaddingVertical(1, Unit.Centimetre).Column(column =>
                {
                    column.Spacing(10);

                    // Title
                    column.Item().AlignCenter().Text("PRODUCTION ORDER").Bold().FontSize(18);

                    // Order Information
                    column.Item().BorderBottom(1).PaddingBottom(5).Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text($"Order Number: {order.OrderNumber}").Bold();
                            col.Item().Text($"Created: {order.CreatedAt:dd MMMM yyyy}");
                            col.Item().Text($"Status: {order.Status}").Bold();
                            col.Item().Text($"Progress: {order.ProgressPercentage}%");
                        });

                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text($"Category: {order.Category?.Name ?? "N/A"}");
                            col.Item().Text($"Deadline: {order.DeadlineDate:dd MMMM yyyy}").Bold();
                            if (order.CompletedAt.HasValue)
                            {
                                col.Item().Text($"Completed: {order.CompletedAt:dd MMMM yyyy}");
                            }
                            if (order.ContractId.HasValue)
                            {
                                col.Item().Text($"Contract: {order.Contract?.ContractNumber ?? "N/A"}");
                            }
                        });
                    });

                    // Customer Information
                    column.Item().PaddingTop(10).Text("Customer Information").Bold().FontSize(14);
                    column.Item().BorderBottom(1).PaddingBottom(5).Column(col =>
                    {
                        col.Item().Text($"Name: {order.Customer?.FullName ?? "N/A"}");
                        col.Item().Text($"Phone: {order.Customer?.PhoneNumber ?? "N/A"}");
                        if (!string.IsNullOrWhiteSpace(order.Customer?.Email))
                        {
                            col.Item().Text($"Email: {order.Customer.Email}");
                        }
                    });

                    // Assignment Information
                    column.Item().PaddingTop(10).Text("Assigned Personnel").Bold().FontSize(14);
                    column.Item().BorderBottom(1).PaddingBottom(5).Column(col =>
                    {
                        if (order.AssignedConstructor != null)
                        {
                            col.Item().Text($"Constructor: {order.AssignedConstructor.FirstName} {order.AssignedConstructor.LastName}");
                        }
                        if (order.AssignedProductionManager != null)
                        {
                            col.Item().Text($"Production Manager: {order.AssignedProductionManager.FirstName} {order.AssignedProductionManager.LastName}");
                        }
                    });

                    // Furniture Types
                    if (order.FurnitureTypes?.Any() == true)
                    {
                        column.Item().PaddingTop(10).Text("Furniture Items").Bold().FontSize(14);
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(30);
                                columns.RelativeColumn();
                                columns.ConstantColumn(80);
                                columns.ConstantColumn(80);
                            });

                            // Header
                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("#");
                                header.Cell().Element(CellStyle).Text("Name");
                                header.Cell().Element(CellStyle).Text("Quantity");
                                header.Cell().Element(CellStyle).Text("Status");
                            });

                            // Rows
                            int index = 1;
                            foreach (var furniture in order.FurnitureTypes)
                            {
                                table.Cell().Element(CellStyle).Text(index++.ToString());
                                table.Cell().Element(CellStyle).Text(furniture.Name);
                                table.Cell().Element(CellStyle).Text("-");
                                table.Cell().Element(CellStyle).Text($"{furniture.ProgressPercentage}%");
                            }
                        });
                    }

                    // Notes
                    if (!string.IsNullOrWhiteSpace(order.Notes))
                    {
                        column.Item().PaddingTop(10).Text("Notes").Bold().FontSize(14);
                        column.Item().BorderBottom(1).PaddingBottom(5).Text(order.Notes);
                    }
                });

                page.Footer().Element(ComposeFooter);
            });
        });

        var pdfBytes = document.GeneratePdf();
        return Task.FromResult(pdfBytes);
    }

    public Task<byte[]> GenerateTechnicalSpecPdfAsync(TechnicalSpecification technicalSpec)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header().Element(ComposeHeader);

                page.Content().PaddingVertical(1, Unit.Centimetre).Column(column =>
                {
                    column.Spacing(10);

                    // Title
                    column.Item().AlignCenter().Text("TECHNICAL SPECIFICATION").Bold().FontSize(18);

                    // Specification Information
                    column.Item().BorderBottom(1).PaddingBottom(5).Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text($"Specification ID: {technicalSpec.Id}").Bold();
                            col.Item().Text($"Created: {technicalSpec.CreatedAt:dd MMMM yyyy}");
                            col.Item().Text($"Status: {(technicalSpec.IsLocked ? "Locked" : "Unlocked")}").Bold();
                        });

                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text($"Furniture: {technicalSpec.FurnitureType?.Name ?? "N/A"}");
                            if (technicalSpec.CompletedAt.HasValue)
                            {
                                col.Item().Text($"Completed: {technicalSpec.CompletedAt:dd MMMM yyyy}");
                            }
                        });
                    });

                    // Furniture Information
                    if (technicalSpec.FurnitureType != null)
                    {
                        column.Item().PaddingTop(10).Text("Furniture Details").Bold().FontSize(14);
                        column.Item().BorderBottom(1).PaddingBottom(5).Column(col =>
                        {
                            col.Item().Text($"Name: {technicalSpec.FurnitureType.Name}");
                            col.Item().Text($"Progress: {technicalSpec.FurnitureType.ProgressPercentage}%");
                            if (!string.IsNullOrWhiteSpace(technicalSpec.FurnitureType.Notes))
                            {
                                col.Item().Text($"Notes: {technicalSpec.FurnitureType.Notes}");
                            }
                        });
                    }

                    // Technical Notes
                    column.Item().PaddingTop(10).Text("Technical Notes").Bold().FontSize(14);
                    column.Item().BorderBottom(1).PaddingBottom(5).MinHeight(100)
                        .Text(technicalSpec.Notes ?? "No technical notes provided.");

                    // Disclaimer
                    column.Item().PaddingTop(20).Text($"Note: This technical specification document is {(technicalSpec.IsLocked ? "LOCKED" : "UNLOCKED")} and {(technicalSpec.IsLocked ? "cannot" : "can")} be modified.").FontSize(9).Italic();

                    // Approval Section
                    column.Item().PaddingTop(30).Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("Technical Engineer").FontSize(10);
                            col.Item().PaddingTop(20).BorderTop(1).Width(150).Text("Signature & Date");
                        });

                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("Production Manager").FontSize(10);
                            col.Item().PaddingTop(20).BorderTop(1).Width(150).Text("Signature & Date");
                        });
                    });
                });

                page.Footer().Element(ComposeFooter);
            });
        });

        var pdfBytes = document.GeneratePdf();
        return Task.FromResult(pdfBytes);
    }

    private void ComposeHeader(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column.Item().Text(CompanyName).Bold().FontSize(16).FontColor(Colors.Blue.Darken2);
                column.Item().Text(CompanyAddress).FontSize(9);
                column.Item().Text($"Phone: {CompanyPhone} | Email: {CompanyEmail}").FontSize(9);
            });

            row.ConstantItem(100).AlignRight().Height(50).Text("");
        });

        container.PaddingTop(5).BorderBottom(2).BorderColor(Colors.Blue.Darken2);
    }

    private void ComposeFooter(IContainer container)
    {
        container.BorderTop(1).PaddingTop(5).AlignCenter()
            .Text($"Generated on: {DateTime.Now:dd MMMM yyyy HH:mm} | FurniFlow Uz").FontSize(9);
    }

    private IContainer CellStyle(IContainer container)
    {
        return container.Border(1).BorderColor(Colors.Grey.Lighten2).Padding(5);
    }
}
