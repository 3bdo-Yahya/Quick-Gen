using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Quick_Gen.Models;

namespace Quick_Gen.Infrastructure;

public static class CertificatePdfGenerator
{
    public static byte[] Generate(Certificate cert)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(40);
                page.Background().Background(Color.FromHex("#FAFAFA"));

                page.Content().Column(col =>
                {
                    // ── Header ────────────────────────────────
                    col.Item().AlignCenter().Text("CERTIFICATE OF COMPLETION")
                        .FontSize(28).Bold().FontColor(Color.FromHex("#1A1A2E"));

                    col.Item().Height(10);

                    col.Item().AlignCenter()
                        .Text("This is to certify that")
                        .FontSize(14).FontColor(Color.FromHex("#555555"));

                    col.Item().Height(15);

                    // ── Name ──────────────────────────────────
                    col.Item().AlignCenter()
                        .Text(cert.RecipientDisplayName ?? "Unknown")
                        .FontSize(36).Bold().FontColor(Color.FromHex("#E94560"));

                    col.Item().Height(15);

                    col.Item().AlignCenter()
                        .Text("has successfully completed the course")
                        .FontSize(14).FontColor(Color.FromHex("#555555"));

                    col.Item().Height(10);

                    // ── Course Title ──────────────────────────
                    col.Item().AlignCenter()
                        .Text($"\"{cert.CourseTitle}\"")
                        .FontSize(22).Bold().FontColor(Color.FromHex("#1A1A2E"));

                    col.Item().Height(30);

                    // ── Footer ────────────────────────────────
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().AlignLeft().Column(c =>
                        {
                            c.Item().Text($"Issued: {cert.IssuedAt:MMMM dd, yyyy}")
                                .FontSize(11).FontColor(Color.FromHex("#888888"));
                        });

                        row.RelativeItem().AlignRight().Column(c =>
                        {
                            c.Item().Text($"Certificate No: {cert.CertificateNumber}")
                                .FontSize(11).FontColor(Color.FromHex("#888888"));
                        });
                    });
                });
            });
        }).GeneratePdf();
    }
}