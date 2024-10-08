﻿using System;

namespace Example
{
    public class Delegate
    {
        delegate void Operation(float a, float b);
        delegate void EventHandler(string message);

        public void DelegateExample()
        {
            Operation operation = new Operation(Addition);
            operation(1, 2);

            operation += new Operation(Subtraction);
            operation(1, 5);

            operation += new Operation(Multiplication);
            operation(1, 8);

            operation += new Operation(Division);
            operation(2, 1);

            operation -= Addition;
            operation.Invoke(2, 0);

            LaunchOperation(operation);

            EventPublisher eventPublisher = new EventPublisher();
            EventSubscriber eventSubscriber = new EventSubscriber();

            eventPublisher.OnMessageSent += eventSubscriber.HandleMessage;
            eventPublisher.SendMessage("DELEGATE");
        }

        private void Addition(float a, float b)
        {
            Console.WriteLine(a + b);
        }

        private void Subtraction(float a, float b)
        {
            Console.WriteLine(a - b);
        }

        private void Multiplication(float a, float b)
        {
            Console.WriteLine(a * b);
        }

        private void Division(float a, float b)
        {
            if (b != 0)
                Console.WriteLine(a / b);
            else
                Console.WriteLine("Invalid Operation");
        }

        private void LaunchOperation(Operation operation)
        {
            operation(5, 2);
        }

        class EventPublisher
        {
            public event EventHandler OnMessageSent;

            public void SendMessage(string message)
            {
                Console.WriteLine($"Sending message: {message}");

                OnMessageSent?.Invoke(message);
            }
        }

        class EventSubscriber
        {
            public void HandleMessage(string message)
            {
                Console.WriteLine($"Received message: {message}");
            }
        }
    }
}
