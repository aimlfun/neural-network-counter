using System.Diagnostics;

namespace NeuralNetworkToCount
{
    /// <summary>
    /// Form to demonstrate counting.
    /// </summary>
    public partial class Form1 : Form
    {
        /// <summary>
        /// Counting is based on a JK flip-flop-counter.
        /// </summary>
        private readonly Counter counter;

        /// <summary>
        /// Determines whether we ask the neural network to increment or decrement.
        /// </summary>
        bool increment = false;

        /// <summary>
        /// Constructor.
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            Show();

            Application.DoEvents(); // enable it to paint "please wait"

            counter = new();
            labelTraining.Visible = false; // hide the please wait

            timer1.Enabled = true; // enable AFTER training
        }

        /// <summary>
        /// Every second, either increment or decrement the counter.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer1_Tick(object sender, EventArgs e)
        {
            counter.Feedback(increment ? 0 : 1);
          
            // display digit (0/1) representing 10s
            pictureBox7SegmentDisplayDigits10to15.Image?.Dispose();
            pictureBox7SegmentDisplayDigits10to15.Image = SevenSegmentDisplay.Output(counter.Value / 10);

            // display digit (0-9)
            pictureBox7SegmentDisplayDigits0to9.Image?.Dispose();
            pictureBox7SegmentDisplayDigits0to9.Image = SevenSegmentDisplay.Output(counter.Value % 10);

            // switch direction, otherwise the counter will wrap around.
            if (counter.Value == 15 || counter.Value == 0) increment = !increment;
        }
    }
}