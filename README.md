# MontarDiscos
aplicacion para simular en Windows el efecto de Linux o MAC OSX de que aparezca la unidad conectada por usb en el escritorio

Soy una persona nueva trabajando con linux, aunque en mi vida e toqueteado este SO de vez en cuando, sin embargo una cosa que
más me gustan de lo poco que he usado Linux y que además e visto en videos en SO Mac OSX a lo largo de mi vida es que cuando 
montas una unidad, ya sea extraíble o de cd/dvd, nos pone automáticamente un icono para acceder a él en el escritorio.

En Windows es totalmente diferente, de hecho la única forma de acceder a estos es a través de Mi PC o el explorador de archivos.
En algún momento pensé que se podría simplemente creando un acceso directo a dicha unidad, lamentablemente fue inviable porque
cuando se desconecta la unidad sigue el acceso directo de la unidad en el escritorio y es posible que conectemos diferentes
tipos (pendrive, reproductor multimedia, etc) a lo largo del día y muy probablemente se le asigne el mismo icono y acceso 
directo que ya tenemos en el escritorio.

Por las razones expuestas anteriormente decidí buscar una alternativa o quizás una opción oculta que tuviera el windows 
que yo no supiera, cuando agote mis esfuerzos de búsqueda de esta opción, oculta tome la decisión de crear una aplicación que 
lo emulara. Esta aplicación comencé a crearla en 2016, la utilice por un tiempo, con algunas fallas, pero lamentablemente
deje su desarrollo en el olvido

Ejecutada se encuentra en constante revisión de unidades conectadas o desconectadas, en lo que a unidades se refiere, 
y si detecta una unidad conectada nos crea un acceso directo a dicha unidad en el escritorio, de lo contrario elimina el 
acceso directo, de hecho si eliminamos el acceso directo del escritorio o lo arrastramos a la papelera la aplicación 
expulsa la unidad del PC

MontarDiscos tiene licencia MIT, requiere .NET 2.0 y está en español.

# MontarDiscos
application to simulate in Windows the effect of Linux or MAC OSX that the drive connected by usb appears on the desktop

I am a new person working with linux, although in my life I have touched this OS from time to time, nevertheless one thing that
The more I like how little I have used Linux and that I have also seen in videos on Mac OSX throughout my life is that when
You mount a drive, either removable or cd / dvd, it automatically puts us an icon to access it on the desktop.

In Windows it is totally different, in fact the only way to access these is through My Computer or the file explorer.
At some point I thought that you could just create a direct access to this unit, unfortunately it was not feasible because
when the unit is disconnected it follows the shortcut of the unit on the desktop and we may connect different
types (pendrive, multimedia player, etc.) throughout the day and most likely will be assigned the same icon and access
direct that we already have on the desktop.

For the reasons stated above I decided to look for an alternative or perhaps a hidden option that had the windows
that I did not know, when I exhaust my efforts to search for this option, I made the decision to create an application that
emulate it. I started creating this application in 2016, I used it for a while, with some flaws, but unfortunately
leave your development in oblivion

Executed is in constant revision of connected or disconnected units, as far as units are concerned,
and if it detects a connected unit it creates a direct access to said unit on the desktop, otherwise it eliminates the
shortcut, in fact if we remove the shortcut from the desktop or drag it to the trash the application
eject drive from PC

MontarDiscos is MIT licensed, requires .NET 2.0 and is in Spanish.
