## Docker
- uzitecny docker ps - ukaze bezici containery
- docker up -d spusti image
- pro pristup do databaze je potreba se dostat do shellu dockeru pomoci: docker exec -it (interactive - zustava prihlasene i po execute commandu, t - formatuje output at to vyapda jako terminal) simple_api_database_mock_container bash
- /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P Dochazka123 -C  --- spusti command line databaze, -C je kvuli konteineru, ktery pouziva self signed ceritifikat pri komunikaci, takze by SQL server odmital requesty


Action	        Command / Query
- Execute	    GO (Must be on a new line)
- Switch DB	    USE [DBName]; GO
- List DBs	    SELECT name FROM sys.databases; GO
- List Tables	SELECT name FROM sys.tables; GO
- Clear Screen	!! clear (Linux) or !! cls (Windows)
- Quit	        QUIT