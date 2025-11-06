using System.Collections.Generic;

namespace Xareus.Unity
{
    /// <summary>
    /// Reports the user activity.
    /// </summary>
    public static class UFEOtherUserActivity
    {
        #region Statics

        /// <summary>
        /// Maps the control index with their owner.
        /// </summary>
        private static readonly Dictionary<int, string> indexOwnerMap = new Dictionary<int, string>();

        #endregion

        #region Methods

        /// <summary>
        /// Sets the owner of a control.
        /// </summary>
        /// <param name="index">The index of the control</param>
        /// <param name="owner">The identifier of the owner or <c>null</c> whether there is no owner</param>
        public static void SetControlOwner(int index, string owner)
        {
            if (owner != null)
            {
                indexOwnerMap[index] = owner;
            }
            else
            {
                indexOwnerMap.Remove(index);
            }
        }

        /// <summary>
        /// Gets the owner of a control.
        /// </summary>
        /// <param name="index">The index of the control</param>
        /// <returns>The identifier of the owner or <c>null</c> whether there is no owner</returns>
        public static string GetControlOwner(int index)
        {
            return (indexOwnerMap.ContainsKey(index) ? indexOwnerMap[index] : null);
        }

        #endregion
    }
}
