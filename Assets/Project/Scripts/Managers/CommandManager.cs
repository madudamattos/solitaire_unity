using System;
using System.Collections.Generic;
using UnityEngine;

namespace Solitaire.Managers
{
    /// <summary>
    /// Defines the contract for encapsulating a reversible action within the Command Pattern.
    /// </summary>
    public interface ICommand
    {
        /// <summary> Executes the forward action of the command. </summary>
        void Execute();
        
        /// <summary> Reverts the action previously executed by the command. </summary>
        void Undo();
    }

    /// <summary>
    /// Static manager responsible for tracking the history of executed commands, 
    /// providing global Undo functionality, and preventing input overlap during card animations.
    /// </summary>
    public static class CommandManager
    {
        private static Stack<ICommand> _commandHistory = new Stack<ICommand>();
        
        private static float _lastCommandTime = 0f;
        private const float AnimationDuration = 0.25f; 
        
        /// <summary> 
        /// Checks if a command is currently within its animation execution window to prevent input spamming. 
        /// </summary>
        public static bool IsProcessing => Time.time < _lastCommandTime + AnimationDuration;
        
        public static bool HasCommands => _commandHistory.Count > 0;

        public static event Action OnCommandExecuted;
        
        public static event Action OnCommandUndone;
        
        /// <summary> Event triggered when the entire command history is wiped. </summary>
        public static event Action OnHistoryCleared;
        
        /// <summary>
        /// Executes the given command immediately and pushes it onto the history stack. 
        /// Locks input temporarily to allow physical animations to play out.
        /// </summary>
        /// <param name="command">The command instance to be executed.</param>
        public static void AddCommand(ICommand command)
        {
            if (IsProcessing) return;

            command.Execute();
            _commandHistory.Push(command);
        
            _lastCommandTime = Time.time;

            OnCommandExecuted?.Invoke();
        }

        /// <summary>
        /// Pops the most recent command from the history stack and executes its <c>Undo</c> logic.
        /// Ignores the request if an animation is currently processing or if the stack is empty.
        /// </summary>
        public static void UndoLastCommand()
        {
            if (_commandHistory.Count == 0 || IsProcessing) return;
            
            ICommand lastCommand = _commandHistory.Pop();
            lastCommand.Undo();

            _lastCommandTime = Time.time;

            OnCommandUndone?.Invoke();
        }

        /// <summary>
        /// Empties the command history stack. Typically called when a new game starts 
        /// or when entering game states where undoing is prohibited (like Auto-Complete).
        /// </summary>
        public static void ClearHistory()
        {
            _commandHistory.Clear();
            OnHistoryCleared?.Invoke();
        }
    }
}