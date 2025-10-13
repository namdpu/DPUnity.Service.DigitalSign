using DigitalSignService.Common;
using DigitalSignService.DAL.Entities;

namespace DigitalSignService.DAL
{
    public class SeedPaperSize
    {
        public static async Task SeedData(DataBaseContext context)
        {
            if (!context.PaperSizes.Any())
            {
                var paperSize = new List<PaperSize> {
                    new PaperSize { Id = Guid.NewGuid(), PaperName = "A0", Width = 841, Height = 1189, PaperSizeType = Enums.PaperSizeType.A0 },
                    new PaperSize { Id = Guid.NewGuid(), PaperName = "A1", Width = 594, Height = 841, PaperSizeType = Enums.PaperSizeType.A1 },
                    new PaperSize { Id = Guid.NewGuid(), PaperName = "A2", Width = 420, Height = 594, PaperSizeType = Enums.PaperSizeType.A2 },
                    new PaperSize { Id = Guid.NewGuid(), PaperName = "A3", Width = 297, Height = 420, PaperSizeType = Enums.PaperSizeType.A3 },
                    new PaperSize { Id = Guid.NewGuid(), PaperName = "A4", Width = 210, Height = 297, PaperSizeType = Enums.PaperSizeType.A4 },
                    new PaperSize { Id = Guid.NewGuid(), PaperName = "A5", Width = 148, Height = 210, PaperSizeType = Enums.PaperSizeType.A5 },
                    new PaperSize { Id = Guid.NewGuid(), PaperName = "A6", Width = 105, Height = 148, PaperSizeType = Enums.PaperSizeType.A6 },
                    new PaperSize { Id = Guid.NewGuid(), PaperName = "A7", Width = 74, Height = 105, PaperSizeType = Enums.PaperSizeType.A7 },
                    new PaperSize { Id = Guid.NewGuid(), PaperName = "A8", Width = 52, Height = 74, PaperSizeType = Enums.PaperSizeType.A8 },
                    new PaperSize { Id = Guid.NewGuid(), PaperName = "A9", Width = 37, Height = 52, PaperSizeType = Enums.PaperSizeType.A9 },
                    new PaperSize { Id = Guid.NewGuid(), PaperName = "A10", Width = 26, Height = 37, PaperSizeType = Enums.PaperSizeType.A10 }
                };
                context.PaperSizes.AddRange(paperSize);
                await context.SaveChangesAsync();
            }
        }
    }
}
