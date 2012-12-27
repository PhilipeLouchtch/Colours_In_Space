using Microsoft.Kinect;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace ColoursInSpace
{
    class VoiceRecognition
    {
        RuntimeSettings settings;

        KinectSensor sensor;
        SpeechRecognitionEngine speechEngine;

        public VoiceRecognition()
        {
			this.settings = RuntimeSettings.Instance;
        }

        public void IntializeRecognition(ref KinectSensor sensor)
        {
            this.sensor = sensor;
            RecognizerInfo recognizerInfo = GetKinectRecognizer();
            if (recognizerInfo != null)
            {
                speechEngine = new SpeechRecognitionEngine(recognizerInfo);

                Choices category = new Choices();
                Choices command = new Choices();

                foreach (string word in this.CategoryMatchings.Keys)
                    category.Add(word);

                foreach (string word in this.CommandMatchings.Keys)
                    command.Add(word);

                /* Build the grammer that is to be recognized */
                GrammarBuilder grammarBuilder = new GrammarBuilder();
                grammarBuilder.Culture = recognizerInfo.Culture;

                grammarBuilder.Append("Kinect");
                grammarBuilder.Append(category);
                grammarBuilder.Append(command);

                Grammar grammar = new Grammar(grammarBuilder);
                speechEngine.LoadGrammar(grammar);
                speechEngine.SpeechRecognized += SpeechRecognized;


                /* Start Input stream and bind it to the recognizer */
                KinectAudioSource audioSource = this.sensor.AudioSource;
                audioSource.BeamAngleMode = BeamAngleMode.Adaptive;

                Stream kinectStream = audioSource.Start();
                speechEngine.SetInputToAudioStream(kinectStream, new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
                speechEngine.RecognizeAsync(RecognizeMode.Multiple);
            }
            else throw new ApplicationException("Failed to initialize the Speech Recognition");
        }

        void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Confidence > 0.6f)
            {
                var result = e.Result.Text;

                if (result.Contains("Filter"))
                {
                    if (result.Contains("Next"))
                    {
                        throw new NotImplementedException("Filters not yet implemented");
                    }
                }
                else if (result.Contains("Volume"))
                {
                    if (result.Contains("Up"))
                    {
						settings.volume += 10;
                    }
                    else if (result.Contains("Down"))
                    {
						settings.volume -= 10;
                    }
                }
                else if (result.Contains("Targets"))
                {
                    if (result.Contains("Up"))
                    {
                        settings.amntTargetBoxes += 2;
                    }
                    else if (result.Contains("Down"))
                    {
                        settings.amntTargetBoxes -= 2;
                    }
                }
                else if (result.Contains("Mode"))
                {
                    if (result.Contains("Sonar"))
                    {
                        throw new NotImplementedException("Mode switching not implemented");
                    }
                    else if (result.Contains("Color"))
                    {
                        throw new NotImplementedException("Mode switching not implemented");
                    }
                    else if (result.Contains("Mixed"))
                    {
                        throw new NotImplementedException("Mode switching not implemented");
                    }
                }
                else if (result.Contains("Zoom"))
                {
                    if (result.Contains("On"))
                    {
                        settings.zoom = true;
                    }
                    else if (result.Contains("Off"))
                    {
                        settings.zoom = false;
                    }
                }
                   
            }
        }

        /// <summary>
        /// Gets the metadata for the speech recognizer (acoustic model) most suitable to
        /// process audio from Kinect device.
        /// </summary>
        /// <returns>
        /// RecognizerInfo if found, <code>null</code> otherwise.
        /// </returns>
        private static RecognizerInfo GetKinectRecognizer()
        {
            foreach (RecognizerInfo recognizer in SpeechRecognitionEngine.InstalledRecognizers())
            {
                string value;
                recognizer.AdditionalInfo.TryGetValue("Kinect", out value);
                if ("True".Equals(value, StringComparison.OrdinalIgnoreCase) && "en-US".Equals(recognizer.Culture.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return recognizer;
                }
            }
            
            return null;
        }

        #region Keywords and Mappings
        private enum Headers
        {
            Filter,
            Volume,
            Targets,
            Mode,
            Zoom
        }

        private enum Command
        {
            Up,
            Down,
            Next,
            Sonar,
            Colour,
            Mixed,
            On,
            Off
        }

        private Dictionary<string, Headers> CategoryMatchings = new Dictionary<string,Headers>
        {
            { "Filter",  Headers.Filter },
            { "Volume",  Headers.Volume },
            { "Targets", Headers.Targets },
            { "Mode",    Headers.Mode },
            { "Zoom",    Headers.Zoom }
        };

        private Dictionary<string, Command> CommandMatchings = new Dictionary<string, Command>
        {
            { "Up",     Command.Up },
            { "Down",   Command.Down },
            { "Next",   Command.Next },
            { "Sonar",  Command.Sonar },
            { "Color",  Command.Colour }, //enUS localization
            { "Mixed",  Command.Mixed },
            { "Off",    Command.Off },
            { "On",     Command.On }
        };

        #endregion

    }
}
