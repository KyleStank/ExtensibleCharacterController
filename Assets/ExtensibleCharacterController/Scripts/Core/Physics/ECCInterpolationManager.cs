using System.Collections.Generic;

using ExtensibleCharacterController.Characters;
using ExtensibleCharacterController.Core.Utility;

namespace ExtensibleCharacterController.Core.Physics
{
    /// <summary>
    /// Handles interpolation of characters using ExtensibleCharacterController.
    /// </summary>
    public class ECCInterpolationManager : ECCSingleton<ECCInterpolationManager>
    {
        private List<ECCCharacter> m_Characters = new List<ECCCharacter>();
        /// <summary>
        /// Returns list of all ECCCharacters that will be handled by ECC's interpolation manager.
        /// </summary>
        /// <value></value>
        public static List<ECCCharacter> Characters
        {
            get => m_Instance.m_Characters;
        }

        /// <summary>
        /// Adds a character to ECC's interpolation manager.
        /// </summary>
        /// <param name="character">Character to add.</param>
        public static void AddCharacter(ECCCharacter character) => Instance.m_Characters.Add(character);

        /// <summary>
        /// Removes a character from ECC's interpolation manager.
        /// </summary>
        /// <param name="character">Character to remove.</param>
        public static void RemoveCharacter(ECCCharacter character) => Instance.m_Characters.Remove(character);

        // TODO: Implement custom interpolation.
    }
}
