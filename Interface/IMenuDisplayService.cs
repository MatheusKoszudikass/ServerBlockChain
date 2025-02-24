using ServerBlockChain.Entities.Enum;

namespace ServerBlockChain.Interface
{
    public interface IMenuDisplayService
    {
        /// <summary>
        /// Registers an option in the menu with the corresponding action.
        /// </summary>
        /// <param name="option">The number of the option.</param>
        /// <param name="action">The action to be executed when the option is selected.</param>
        void RegisterOption(int option, Action action);

        /// <summary>
        /// Removes all registered options from the menu.
        /// </summary>
        void DeleteOption();

        /// <summary>
        /// Displays the menu based on the specified type.
        /// </summary>
        /// <param name="type">The type of menu to be displayed.</param>
        void DisplayMenu(TypeHelp type);

        /// <summary>
        /// Displays the menu based on the specified type and a list of options.
        /// </summary>
        /// <param name="type">The type of menu to be displayed.</param>
        /// <param name="array">The list of options to be displayed.</param>
        void DisplayMenu(TypeHelp type, object[] array);

        /// <summary>
        /// Displays the menu based on the specified type, a list of options, and a selected index.
        /// </summary>
        /// <param name="type">The type of menu to be displayed.</param>
        /// <param name="array">The list of options to be displayed.</param>
        /// <param name="selectedIndex">The index of the selected option.</param>
        void DisplayMenu(TypeHelp type, object[] array, int selectedIndex);

        void DisplayMenu(TypeHelp type, string[] array, object[] objs, int selectedIndex);

        /// <summary>
        /// Executes the action corresponding to the selected option.
        /// </summary>
        /// <param name="selectedOption">The number of the selected option.</param>
        void ExecuteOption(int selectedOption);

        /// <summary>
        /// Displays the menu based on the specified type and a list of options.
        /// </summary>
        /// <param name="type">The type of menu to be displayed.</param>
        /// <param name="options">The list of options to be displayed.</param>
        void SelectedView(TypeHelp type, string[] options);

        /// <summary>
        /// Displays the menu and returns the option selected by the user.
        /// </summary>
        /// <param name="type">The type of menu to be displayed.</param>
        /// <param name="array">The list of options to be displayed.</param>
        /// <param name="objs">Additional objects for display.</param>
        /// <returns>The option selected by the user.</returns>
        object? SelectedOption(TypeHelp type, string[] array, object[] objs);

        /// <summary>
        /// Returns the index of the option selected by the user.
        /// </summary>
        /// <returns>The index of the selected option.</returns>
        int SelectedIndex();
    }
}