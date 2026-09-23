using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace Box_collider.Helpers
{
    public static class KeyboardManager
    {
        // Cache the list of keys we care about to avoid iterating the dictionary itself during Update
        private static readonly List<Keys> registeredPressedKeys = new();
        private static readonly List<Keys> registeredHeldKeys = new();

        private static readonly Dictionary<Keys, List<Action>> pressedActions = new();
        private static readonly Dictionary<Keys, List<Action>> heldActions = new();

        private static KeyboardState currentState;
        private static KeyboardState previousState;

        // Runs once when the key is initially pressed
        public static void RegisterPressed(Keys key, Action action)
        {
            if (!pressedActions.TryGetValue(key, out var list))
            {
                list = new List<Action>();
                pressedActions[key] = list;
                registeredPressedKeys.Add(key); // Cache the key
            }
            list.Add(action);
        }

        // Runs every frame while the key is held
        public static void RegisterHeld(Keys key, Action action)
        {
            if (!heldActions.TryGetValue(key, out var list))
            {
                list = new List<Action>();
                heldActions[key] = list;
                registeredHeldKeys.Add(key); // Cache the key
            }
            list.Add(action);
        }

        public static bool IsHeld(Keys key)
        {
            return currentState.IsKeyDown(key);
        }

        public static bool AreHeld(params Keys[] keys)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                if (!currentState.IsKeyDown(keys[i]))
                    return false;
            }

            return true;
        }

        public static void Update()
        {
            previousState = currentState;
            currentState = Keyboard.GetState();

            // 1. Process Pressed Actions (Zero-allocation using for-loops)
            int pressedKeyCount = registeredPressedKeys.Count;
            for (int i = 0; i < pressedKeyCount; i++)
            {
                Keys key = registeredPressedKeys[i];

                if (currentState.IsKeyDown(key) && previousState.IsKeyUp(key))
                {
                    List<Action> actions = pressedActions[key];
                    int actionCount = actions.Count;
                    for (int j = 0; j < actionCount; j++)
                    {
                        actions[j]();
                    }
                }
            }

            // 2. Process Held Actions (Zero-allocation using for-loops)
            int heldKeyCount = registeredHeldKeys.Count;
            for (int i = 0; i < heldKeyCount; i++)
            {
                Keys key = registeredHeldKeys[i];

                if (currentState.IsKeyDown(key))
                {
                    List<Action> actions = heldActions[key];
                    int actionCount = actions.Count;
                    for (int j = 0; j < actionCount; j++)
                    {
                        actions[j]();
                    }
                }
            }
        }
    }
}
