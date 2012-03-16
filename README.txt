Hello fellow visitor !

I present you my project, a Remote Control Server, which works by sending

very simple strings, whose does simple commands.

It can easily be used by any language (programming, heh.), because it's

using sockets.

For a concrete example, in python, I send the string like this:

            Remote_Socket = socket.socket(socket.AF_INET,socket.SOCK_STREAM)

            Remote_Socket.connect((ipAdressofRemoteHost,portOfRemoteHost))

            Remote_Socket.send("query")

I'd try to make some different connectors in different languages, to help

the use of the program.


After that, on the server, we handle the query. You have a XML config file, created

at the start of the process, it reads it, update the information needed, and reads

commands there. Let me explain the system: If the server receives a "magic string", like

"writesomething" in the example config.xml, sent from a client (obviously), it does what 

the command is meant. E.G, if the command is "exec", it'll execute the file that is passed

in argument, we'll take here "C:\notepad.exe". I focus to get a lot of commands, but I'll try,

for the moment, to get the server the most stable possible.

Anyway, for the commands, you can check a basic config.xml at the root of the repo.

You can easily add commands there, but I'll explain which are working,

and how, of course.



For the moment, I only focus Windows, because nearly everything can be

done by commands on Unix systems. It is already included for basic shell

commands like ls, date, etc... They DOES return something, it's sended to 

the client after the query was successful. There's much more things that can

be done easily on unix via commands, so now I focus on windows to get the

more commands possible, including external librairies.

For example, I already included iTunes remote control, i'll use the library at the fullest later.



If any information/help needed, contact me @ Mathysleleu[at]gmail(dot)com !



TODO:

                 - MOAR librairies !

                 - MOAR COMMANDS !

                 

