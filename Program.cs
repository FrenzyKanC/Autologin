using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System;
using System.Collections.Generic;
using System.Threading;


// To-Do List


// b) fehlerhafter ladeprozess !click kein ausloggen sondern button suchen sek takt

// c) anfrage !erfolgreich offen lassen und neues fenster öffnen; prozess weiter laufen lassen bei session timeout fenster schließen


// 2. Es fehlt die Logik für antragseinreichungErfolgreich = true --> geht erst wenn die Seitenstruktur bekannt ist (click feld-> absenden)
// a) indexi testen für mögliche folgemaske
// ähnliche portale nach button name durchforsten

// 4. Abwägung:
// a) mehrere browser öffnen testen (mehrere user)
//    - Mehrere Programme laufen lassen mit Single User, dadurch entsteht gegebenenfalls 5.
//          --->array streichen und einzeldatensatz hinterlegen

// 5a. User Interface erstellen, wenn Software für Kunden sein soll bzw. für Programmierfremde-User

// 5b. Datenbank mit MVC ausarbeiten; könnte das Interface sein

// 6. Exe für Programmierfremde-User erstellen:
/*  Öffnen Sie Ihr C#-Projekt in Visual Studio:
    Starten Sie Visual Studio und öffnen Sie Ihr C#-Projekt.

    Stellen Sie sicher, dass Ihre Anwendung funktioniert:
    Führen Sie sicher, dass Ihre Anwendung ohne Fehler kompiliert und ausgeführt werden kann. Stellen Sie sicher, dass alle Abhängigkeiten korrekt konfiguriert sind.

    Ändern Sie die Build-Konfiguration:
    Wählen Sie die Build-Konfiguration aus, die Sie für Ihre Anwendung verwenden möchten (z. B. "Release"). Dies kann in der Symbolleiste von Visual Studio ausgewählt werden.

    Veröffentlichen Sie Ihr Projekt:
    Gehen Sie zu "Build" in der Menüleiste, wählen Sie "Publish [Ihr Projektname]".

    Wählen Sie das Veröffentlichungsziel:
    Wählen Sie das Veröffentlichungsziel aus. Wenn Sie eine eigenständige .exe-Datei erstellen möchten, können Sie "Folder" oder "File" auswählen und dann das Verzeichnis, in dem die Dateien gespeichert werden sollen.

    Veröffentlichen Sie Ihr Projekt:
    Klicken Sie auf "Publish", um Ihr Projekt zu veröffentlichen. Die ausführbare Datei und alle erforderlichen Dateien werden im ausgewählten Verzeichnis gespeichert.

Dies erstellt eine ausführbare Datei sowie alle erforderlichen Dateien, die für die Ausführung Ihrer Anwendung notwendig sind. Stellen Sie sicher, dass Sie die gesamte Ordnerstruktur mit den Abhängigkeiten auf das Zielsystem kopieren, wenn Sie die Anwendung dort ausführen möchten. */




class Programm
{
    static void Main()
    {
        var driverPath = @"C:\Users\stkan\OneDrive\Desktop\geckodriver-v0.34.0-win64";

        var userList = new List<(string Username, string Password)>
        {
            ("Testaccount", "Testaccount123!"),
            // Füge weitere Benutzer hinzu, falls notwendig
        };

        var driver = new FirefoxDriver(driverPath);

        DateTime desiredDateTime = new DateTime(2024, 1, 14, 21, 55, 0);

        while (DateTime.Now < desiredDateTime.AddMinutes(30)) // Setze eine Zeitüberschreitung
        {
            foreach (var user in userList)
            {
                Console.WriteLine($"Versuche die Anmeldung für Benutzer '{user.Username}'.");

                driver.Navigate().GoToUrl("https://antrag.mittelstand-innovativ-digital.nrw/mid-digitale-sicherheit/login");

                var usernameField = driver.FindElement(By.Id("login_name"));
                var passwordField = driver.FindElement(By.Id("password"));
                var loginButton = driver.FindElement(By.CssSelector("button[type='submit']"));

                usernameField.SendKeys(user.Username);

                if (IsElementInteractable(passwordField))
                {
                    passwordField.SendKeys(user.Password);
                }

                loginButton.Click();

                Thread.Sleep(5000);

                if (driver.PageSource.Contains("Willkommen"))
                {
                    Console.WriteLine($"Anmeldung erfolgreich für Benutzer '{user.Username}'.");
                    VersucheAntragseinreichung(driver, user);
                    break;
                }
                else
                {
                    Console.WriteLine($"Anmeldung fehlgeschlagen für Benutzer '{user.Username}'. Wiederhole...");
                    Thread.Sleep(2500);
                }
            }
        }

        driver.Quit();
    }

    static void VersucheAntragseinreichung(IWebDriver driver, (string Username, string Password) user)
    {
        Thread.Sleep(2500);

        var antragseinreichungButton = driver.FindElement(By.CssSelector("li.sidebar-navi-item:nth-child(11) > a:nth-child(1) > span:nth-child(2)"));
        antragseinreichungButton.Click();

        Thread.Sleep(2500);

        // Beispiel für die Überprüfung des Erfolgs der Antragseinreichung
        bool antragseinreichungErfolgreich = driver.PageSource.Contains("Antrag erfolgreich eingereicht");

        if (antragseinreichungErfolgreich)
        {
            Console.WriteLine($"Antragseinreichung erfolgreich für Benutzer '{user.Username}'.");
        }
        else
        {
            Console.WriteLine($"Antragseinreichung fehlgeschlagen für Benutzer '{user.Username}'. Logout wird versucht...");
            VersucheLogout(driver, user);
        }
    }

    static void VersucheLogout(IWebDriver driver, (string Username, string Password) user)
    {
        // Füge hier deine Logout-Logik ein, z.B. Klick auf den Logout-Button und Überprüfung des erfolgreichen Logouts
        // Beispiel:
        var logoutButton = driver.FindElement(By.CssSelector("a[href*='logout']"));
        logoutButton.Click();

        Thread.Sleep(2500);

        // Beispiel für die Überprüfung des Erfolgs des Logouts
        bool logoutErfolgreich = driver.PageSource.Contains("Koordinator login");

        if (logoutErfolgreich)
        {
            Console.WriteLine($"Logout erfolgreich für Benutzer '{user.Username}'.");
        }
        else
        {
            Console.WriteLine($"Logout fehlgeschlagen für Benutzer '{user.Username}'.");
        }
    }

    static bool IsElementInteractable(IWebElement element)
    {
        try
        {
            return element.Enabled && element.Displayed;
        }
        catch (StaleElementReferenceException)
        {
            return false;
        }
    }
}
