using UnityEngine;

namespace Microlight.MicroEditor {
    // ****************************************************************************************************
    // Various constants 
    // ****************************************************************************************************
    internal static class MicroEditor_Utility {
        // Colors
        internal static Color EditorColor = new Color(0.2196f, 0.2196f, 0.2196f);   // Color of editor base background color RGB(56)
        internal static Color ElementColor = new Color(0.251f, 0.251f, 0.251f);   // Color of element background RGB(64)
        internal static Color ElementHoverColor = new Color(0.3176f, 0.3176f, 0.3176f);   // Color of of element when hovering RGB(81)

        internal static Color OutlineColor = new Color(0.1373f, 0.1373f, 0.1373f);   // Outline color RGB(35)
        internal static Color LightOutlineColor = new Color(0.1804f, 0.1804f, 0.1804f);   // Secondary lighter outline color RGB(46)

        internal static Color FontColor = new Color(0.7294f, 0.7294f, 0.7294f);   // Editor font color (lables) RGB(186)
        internal static Color DarkFontColor = new Color(0.3765f, 0.3765f, 0.3765f);   // Darker font color (grayed out) RGB(96)

        // Sizing
        internal const int LineHeight = 18;   // EditorGUIUtility.singleLineHeight
        internal const int HeaderLineHeight = 27;   // Line height * 1.5
        internal const int VerticalSpacing = 2;   // EditorGUIUtility.standardVerticalSpacing;
        internal const int HorizontalSpacing = 6;   // EditorGUI.kDefaultSpacing
        internal const int SingleRowThreshold = 300;   // Minimum size of the inspector view to display 2 fields in 1 row (when specified)
        internal const int UnityTwoRowsThreshold = 330;   // Below which width does unity default fields (vector 2 and 3) start to be split into 2 rows
        internal static float DefaultFieldHeight => LineHeight + VerticalSpacing;
    }
}