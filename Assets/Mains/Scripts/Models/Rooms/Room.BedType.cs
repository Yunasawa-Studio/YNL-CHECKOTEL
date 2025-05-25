namespace YNL.Checkotel
{
    public partial class Room
    {
        [System.Serializable]
        public class BedType
        {
            public BedSize Size = BedSize.Single;
            public BedDesign Design = BedDesign.Futon;
            public BedStyle Style = BedStyle.Traditional;
            public BedKid KidType = BedKid.None;
            public BedFrame Frame = BedFrame.Simple;

        }

        public enum BedSize
        {
            Single,         // Compact bed for one person.
            Twin,           // Narrow bed, common for kids.
            ExtraTwin,      // Slightly larger than Twin.
            Full,           // Wider, fits one comfortably or two snugly.
            Queen,          // Standard double bed.
            King,           // Spacious, luxurious size.
            CalKing         // Extra-long King bed.
        }

        public enum BedDesign
        {
            Day,            // Multi-purpose, sofa by day.
            Futon,          // Foldable, space-saving design.
            Water,          // Fluid-filled mattress.
            Air,            // Inflatable, adjustable firmness.
            Bookcase,       // Built-in shelving.
            Murphy,         // Folds into the wall.
            Convertible,    // Changes form as needed.
            Round,          // Circular, aesthetic.
            Divan,          // Upholstered base, no headboard.
            Ottoman,        // Storage in base.
            Panel,          // Decorative framed style.
            Hanging,        // Suspended from ceiling.
            Hammock,        // Swinging relaxation.
            Upholstered,    // Soft fabric-covered.
            Cot             // Lightweight, portable bed.
        }

        public enum BedStyle
        {
            Traditional,    // Classic, timeless design.
            Rustic,         // Natural wood, countryside feel.
            Country,        // Cozy, farmhouse-inspired.
            Modern,         // Sleek, minimal aesthetic.
            Industrial,     // Metal, rugged look.
            Retro,          // Vintage-inspired design.
            Cottage,        // Soft, charming decoration.
            French,         // Elegant, sophisticated look.
            Woven,          // Intricate woven patterns.
            Scandinavian    // Clean, simple, Nordic style.
        }

        public enum BedKid
        {
            None,           // No bed kid available
            Single,         // Simple bed for a child.
            Bunk,           // Stacked beds for two.
            TripleBunk,     // Three-tier bunk setup.
            FutonBunk,      // Bunk with foldable bottom bed.
            Trundle,        // Hideaway rolling bed.
            Loft,           // Elevated bed with space below.
            Cabin           // Cozy enclosed sleeping space.
        }

        public enum BedFrame
        {
            Simple,         // Basic, functional design.
            Metal,          // Sturdy, industrial material.
            Wooden,         // Classic wooden frame.
            Brass,          // Decorative metal frame.
            Wingback,       // Side panels for a grand look.
            Slat,           // Wooden slats for mattress support.
            Spindle         // Thin vertical rods for decoration.
        }
    }
}