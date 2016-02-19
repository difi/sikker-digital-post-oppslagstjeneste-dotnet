﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Difi.Felles.Utility;

namespace Difi.Oppslagstjeneste.Klient.Testklient
{
    class Program
    {
        static void Main(string[] args)
        {
            var konfigurasjon = new OppslagstjenesteKonfigurasjon(Miljø.FunksjoneltTestmiljø);
            Logger.Initialize(konfigurasjon);
            Logger.Log(TraceEventType.Information, "> Starter program!");

            var avsendersertifikatThumbprint = CertificateIssuedToPostenNorgeAsIssuedByBuypassClass3Test4Ca3();
            

            OppslagstjenesteKlient register = new OppslagstjenesteKlient(avsendersertifikatThumbprint, konfigurasjon);
            
            var endringer = register.HentEndringer(600, 
                Informasjonsbehov.Kontaktinfo);
            
            var personer = register.HentPersoner(new string[] { "08077000292" }, 
                Informasjonsbehov.Sertifikat | 
                Informasjonsbehov.Kontaktinfo | 
                Informasjonsbehov.SikkerDigitalPost);

            var cert = ExportToPEM(personer.ElementAt(0).X509Sertifikat);
           
            var printSertifikat = register.HentPrintSertifikat();
            Console.WriteLine("Ferdig med oppslag ...");
            
            Console.ReadKey();
        }

        private static string CertificateIssuedToPostenNorgeAsIssuedByBuypassClass3Test4Ca3()
        {
            //return "b0cb922214d11e8ce993838db4c6d04c0c0970b8";
            return "8702F5E55217EC88CF2CCBADAC290BB4312594AC";
        }

        private static string CertificateIssuedToDirektoratetForForvaltningOgIktIssuedByBuypassClass3Ca3()
        {
            return "a47d57eaf69b627710fa0d06ec77500baf71c432";
        }

        public static string ExportToPEM(X509Certificate cert)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("-----BEGIN CERTIFICATE-----");
            builder.AppendLine(Convert.ToBase64String(cert.Export(X509ContentType.Cert), Base64FormattingOptions.InsertLineBreaks));
            builder.AppendLine("-----END CERTIFICATE-----");

            return builder.ToString();
        }
    }
}
