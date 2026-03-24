using System.Runtime.CompilerServices;
using NSMedieval.UI;

namespace SmartCarry.Runtime;

internal static class CharacterEditorGenerationState
{
    private static readonly ConditionalWeakTable<CharactersView, GenerationEntry> StateByView = new();

    public static void SetGenerating(CharactersView view, bool isGenerating)
    {
        if (view == null)
        {
            return;
        }

        StateByView.GetOrCreateValue(view).IsGenerating = isGenerating;
    }

    public static bool IsGenerating(CharactersView? view)
    {
        if (view == null)
        {
            return false;
        }

        return StateByView.TryGetValue(view, out var entry) && entry.IsGenerating;
    }

    private sealed class GenerationEntry
    {
        public bool IsGenerating { get; set; }
    }
}
