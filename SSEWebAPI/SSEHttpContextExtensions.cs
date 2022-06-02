using System.Text.Json;

namespace SSEWebAPI
{
    public static class SSEHttpContextExtensions
    {
        public static async Task SSEInitAsync(this HttpContext ctx)
        {
            ctx.Response.Headers.Add("Cache-Control", "no-cache");
            ctx.Response.Headers.Add("Content-Type", "text/event-stream");
            await ctx.Response.Body.FlushAsync();
        }

        public static async Task SSESendDataAsync(this HttpContext ctx, string data)
        {
            data.Split('\n')
                .ForEach(async line => await ctx.Response.WriteAsync($"data: ${line} {Environment.NewLine}"));

            await ctx.ResponseFlushAsync();
        }

        public static async Task SSESendEventAsync(this HttpContext ctx, SSEEvent e)
        {
            if (!string.IsNullOrWhiteSpace(e.Id))
            {
                await ctx.WritePropertyAsync("id", e.Id);
            }

            if (e.Retry is not null)
            {
                await ctx.Response.WriteAsync($"retry: {e.Retry}{Environment.NewLine}");
            }

            await ctx.WritePropertyAsync("event", e.Name);

            var lines = e.Data switch
            {
                null => new[] { string.Empty },
                string s => s.Split('\n').ToArray(),
                _ => new[] { JsonSerializer.Serialize(e.Data) }
            };

            lines.ForEach(async line => await ctx.WritePropertyAsync("data", line));

            await ctx.ResponseFlushAsync();
        }

        public static async Task SSESendCommentAsync(this HttpContext ctx, string comment)
        {
            foreach (var line in comment.Split('\n'))
            {
                await ctx.Response.WriteAsync(": " + line + "\n");
            }

            await ctx.ResponseFlushAsync();
        }

        internal static async Task WritePropertyAsync(this HttpContext ctx, string key, string value)
        {
            await ctx.Response.WriteAsync($"{key}: {value}{Environment.NewLine}");
        }

        internal static async Task ResponseFlushAsync(this HttpContext ctx)
        {
            await ctx.Response.WriteAsync("\n");
            await ctx.Response.Body.FlushAsync();

        }
    }
}
