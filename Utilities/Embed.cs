using Discord;

namespace RailsIO.Utilities
{
    public static class EmbedUtility
    {
        public static EmbedFieldBuilder CreateBlankField(bool inline = false)
        {
           return new EmbedFieldBuilder()
           {
               Name = "\u200b",
               Value = "\u200b",
               IsInline = inline
           };
        }
    }
}
