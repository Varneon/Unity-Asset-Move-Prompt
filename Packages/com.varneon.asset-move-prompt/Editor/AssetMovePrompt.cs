using System;
using System.IO;
using UnityEditor;

namespace Varneon.AssetMovePrompt
{
    /// <summary>
    /// Asset modification processor class for verifying if user intended to move assets in the project
    /// </summary>
    public class AssetMovePrompt : UnityEditor.AssetModificationProcessor
    {
        /// <summary>
        /// The time window for completing all asset move actions
        /// </summary>
        private static DateTime modificationTimeWindow = DateTime.MinValue;

        /// <summary>
        /// Cached move result
        /// </summary>
        private static AssetMoveResult moveResult;

        private static AssetMoveResult OnWillMoveAsset(string sourcePath, string destinationPath)
        {
            // If the file is being renamed in the same folder, bypass the prompt
            if (Path.GetDirectoryName(sourcePath).Equals(Path.GetDirectoryName(destinationPath))) { return AssetMoveResult.DidNotMove; }

            // If this method gets invoked withing the time window, return the cached result
            if(DateTime.Now < modificationTimeWindow) { return moveResult; }

            // Check if the user intended to move the assets
            bool allowedToMoveAssets = EditorUtility.DisplayDialog("Move asset(s)?", "Are you sure you want to move asset(s)?", "Yes", "No");

            // Assign the cached move result
            moveResult = allowedToMoveAssets ? AssetMoveResult.DidNotMove : AssetMoveResult.FailedMove;

            // Add one second to the time window of the current batch modification
            modificationTimeWindow = DateTime.Now.AddSeconds(1);

            return moveResult;
        }
    }
}
