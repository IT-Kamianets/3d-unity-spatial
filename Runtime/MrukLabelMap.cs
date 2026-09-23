#if MRUK_PRESENT
using ITKamianets.Engine.Scene.Model;
using Meta.XR.MRUtilityKit;

namespace ITKamianets.Engine.Spatial
{
    /// <summary>Maps a Meta Quest MRUK anchor's semantic label to the schema's SemanticLabel.</summary>
    public static class MrukLabelMap
    {
        public static SemanticLabel ToSemanticLabel(MRUKAnchor anchor)
        {
            var label = anchor.Label;

            if (label.HasFlag(MRUKAnchor.SceneLabels.FLOOR)) return SemanticLabel.FLOOR;
            if (label.HasFlag(MRUKAnchor.SceneLabels.CEILING)) return SemanticLabel.CEILING;
            if (label.HasFlag(MRUKAnchor.SceneLabels.WALL_FACE)) return SemanticLabel.WALL_FACE;
            if (label.HasFlag(MRUKAnchor.SceneLabels.INVISIBLE_WALL_FACE)) return SemanticLabel.INVISIBLE_WALL_FACE;
            if (label.HasFlag(MRUKAnchor.SceneLabels.DOOR_FRAME)) return SemanticLabel.DOOR_FRAME;
            if (label.HasFlag(MRUKAnchor.SceneLabels.WINDOW_FRAME)) return SemanticLabel.WINDOW_FRAME;
            if (label.HasFlag(MRUKAnchor.SceneLabels.TABLE)) return SemanticLabel.TABLE;
            if (label.HasFlag(MRUKAnchor.SceneLabels.COUCH)) return SemanticLabel.COUCH;
            if (label.HasFlag(MRUKAnchor.SceneLabels.BED)) return SemanticLabel.BED;
            if (label.HasFlag(MRUKAnchor.SceneLabels.STORAGE)) return SemanticLabel.STORAGE;
            if (label.HasFlag(MRUKAnchor.SceneLabels.SCREEN)) return SemanticLabel.SCREEN;
            if (label.HasFlag(MRUKAnchor.SceneLabels.LAMP)) return SemanticLabel.LAMP;
            if (label.HasFlag(MRUKAnchor.SceneLabels.PLANT)) return SemanticLabel.PLANT;
            if (label.HasFlag(MRUKAnchor.SceneLabels.WALL_ART)) return SemanticLabel.WALL_ART;

            return SemanticLabel.OTHER;
        }
    }
}
#endif
