using System;
using System.Collections.Generic;
using HueSeek.Core;
using UnityEngine;

namespace HueSeek.Paint
{
    /// <summary>
    /// Per-player palette with up to 8 swatches, swappable mid-round.
    /// </summary>
    public class PaintPalette
    {
        private readonly PaintSwatch[] _slots = new PaintSwatch[GameConstants.MaxPaletteSlots];
        private int _activeIndex;

        public int ActiveIndex => _activeIndex;
        public int SlotCount => GameConstants.MaxPaletteSlots;

        public PaintSwatch ActiveSwatch => _slots[_activeIndex];

        public IReadOnlyList<PaintSwatch> Slots => _slots;

        public void SetActiveSlot(int index)
        {
            _activeIndex = Mathf.Clamp(index, 0, GameConstants.MaxPaletteSlots - 1);
        }

        public void StoreSwatch(int index, PaintSwatch swatch)
        {
            _slots[Mathf.Clamp(index, 0, GameConstants.MaxPaletteSlots - 1)] = swatch;
        }

        public bool TryGetSlot(int index, out PaintSwatch swatch)
        {
            index = Mathf.Clamp(index, 0, GameConstants.MaxPaletteSlots - 1);
            swatch = _slots[index];
            return swatch.IsValid;
        }
    }
}
