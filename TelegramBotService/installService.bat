@rem Don't forget: encoding -- OEM866, run -- with admin privileges
sc create TelegramBotIntranet binPath= "%~dp0%TelegramBotService.exe" start= auto DisplayName= "TelegramBot.Intranet"