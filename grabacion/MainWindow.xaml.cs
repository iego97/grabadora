﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using NAudio;
using NAudio.Wave;
using NAudio.Dsp;


namespace grabacion
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        WaveIn waveIn;
        WaveFormat formato;
        WaveFileWriter writer;
        AudioFileReader reader;
        WaveOutEvent waveOut;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnIniciar_Click(object sender, RoutedEventArgs e)
        {
            
            waveIn = new WaveIn();
            waveIn.WaveFormat = new WaveFormat(44100, 16, 1);
            formato = waveIn.WaveFormat;


            waveIn.DataAvailable += OnDataAvailable;
            waveIn.RecordingStopped += OnRecordingStopped;

            writer = new WaveFileWriter("sonido2.wav", formato);
            waveIn.StartRecording();
        }

        void OnRecordingStopped(object sender, StoppedEventArgs e)
        {
            writer.Dispose();
        }

        void OnDataAvailable(object sender, WaveInEventArgs e)
        {
            byte[] buffer = e.Buffer;
            int bytesGrabados = e.BytesRecorded;


            double acumulador = 0;
            double numMuestras = bytesGrabados / 2;
            int exponente = 1;
            int numeroMuestasComplejas = 0;
            int bitsMaximos = 0;

            do
            {
                bitsMaximos = (int)Math.Pow(2, exponente);
                exponente++;
            } while (bitsMaximos < (int)numMuestras);

            exponente -= 2;
            numeroMuestasComplejas = bitsMaximos / 2;


            Complex[] muestrasComplejas =
                new Complex[5];

            for (int i = 0; i < bytesGrabados; i+=2)
            {
                // byte i = 0 1 1 0 0 1 1 1 
                //byte 1+i= 0 0 0 0 0 0 0 0 1 0 1 1 0 1 0 1
                // or     = 1 1 0 0 0 1 1 1 1 0 1 1 0 1 0 1
                short muestra = 
                    (short)Math.Abs((buffer[i+1] << 8) | buffer[i]);

                float muestra32bits = (float)muestra / 32768.0f;
                

                sldVolumen.Value = Math.Abs(muestra32bits);
                

                //acumulador += muestra;
                //numMuestras++;


            }

            double promedio = acumulador / numMuestras;
            //sldVolumen.Value = promedio;
            //writer.Write(buffer, 0, bytesGrabados);
        }

        private void btnDetener_Click(object sender, RoutedEventArgs e)
        {
            waveIn.StopRecording();
        }

        private void btnReproducir_Click(object sender, RoutedEventArgs e)
        {
            reader = new AudioFileReader("sonido2.wav");
            waveOut = new WaveOutEvent();
            waveOut.Init(reader);
            waveOut.Play();
         
            
                }
    }
}