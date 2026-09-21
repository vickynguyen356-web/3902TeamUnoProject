using Sprint0.Interfaces;

namespace Sprint0.Input
{
    // Available when the team wants multiple input adapters.
    public class CombinedController : IController
    {
        private readonly IController[] _controllers;

        public CombinedController(params IController[] controllers)
        {
            _controllers = new IController[controllers.Length];
            for (int controllerIndex = 0; controllerIndex < controllers.Length; controllerIndex++)
            {
                _controllers[controllerIndex] = controllers[controllerIndex];
            }
        }

        public void Update()
        {
            foreach (IController controller in _controllers)
            {
                controller.Update();
            }
        }
    }
}
