API Programming
===============

# Dokumentation

* JSON: Installation:  Install-Package Newtonsoft.Json in Tools->Bibliothek-Paket-Manager->Paket-Manager-Konsole zur Installaion der JSON deserializer und serializer

JSON
```json
	{"id":"ID","method":"authenticate","params":{"user":"ANDROID","password":"PASSWORD", "client":"CLIENT"},"jsonrpc":"2.0"}
```
Login
```json
	{"id":"ID","method":"authenticate","params":{"user":"**********","password":"******", "client":"CLIENT"},"jsonrpc":"2.0"}
```
Result
```json
	{"jsonrpc":"2.0","id":"ID","result":{"sessionId":"***************","personType":0,"personId":-1}}
```


#Wichtig

Methode: HTTP POST

PATH funktioniert nicht -> Cookie-Methode verwenden