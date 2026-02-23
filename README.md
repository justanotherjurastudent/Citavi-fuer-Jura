# Citavi-fuer-Jura
Dieses Projekt enthält den Zitierstil für Jura - "Juristisches Zitieren (Collier)" - und darüber hinaus auch alle entwickelten Codes für Komponenten und Vorlagen.

Kurze Hinweise: Es sei hier nochmals darauf hingewiesen, dass viele Lehrkräfte unterschiedliche Zitierstile bevorzugen. 
Dies kann ich leider nicht alles in einem Stil unterbringen. Daher solltest du dich mit dem 
Programm dahingehend vertraut machen, dass du ggf. selbst Änderungen in dem Baukastensystem vornehmen kannst.
Falls du Fehler bemerken solltest oder mal nicht weißt, wie du etwas anpasst, 
kannst du mich gerne unter der E-Mail-Adresse auf meinem Blog erreichen.

-- Version 2.0.0 --
- Die Seiten- und Auflagenausgabe passt sich nun automatisch an die Sprachen Deutsch, Englisch, Französisch, Italienisch und Spanisch an.
- Die Ausgabe der Beiträge von Kommentaren, Sammelwerken, Tagungsbänden wurde um die Herausgeber des übergeordneten Werkes erweitert. Dies ermöglicht das schnellere Finden des Titels im Literaturverzeichnis.
- Die Herausgeber werden in den Literaturverzeichnisangaben zu Beiträgen in Sammelwerken nun nach dem Muster „Nachname, Vorname“ ausgegeben.
- Die Parlamentsdrucksachen wurden um Plenarprotokolle erweitert.
- Die Ausgabe eines Gesetzes wurde um die Seitenangabe am Ende erweitert.
BeckRS hat eine eigene Vorlage bei den Gerichtsentscheidungen für einen optimalen Nachweis erhalten.
- Die Ausgabe von Pressemitteilungen wurde aktualisiert.
- Die In-Text-Nachweise wurden überarbeitet und erweitert.
- Die Anleitungen zu den Gesetzen, Parlamentsdrucksachen, Gerichtsentscheidungen und  Interdokumenten wurde aktualisiert.

-- Version 2.0.1 --
Die Ausgabe von Fußnoten- und Literaturverzeichniseinträgen für Gesetzeskommentare mit Sachtiteln wurde verbessert. Die Großkommentare zum HGB von Staub („Großkomm. HGB“) sowie zum AktG (Großkomm. AktG“) wurden zur Liste der Sachtitel hinzugefügt.

-- Version 2.0.2 --
Für Gerichtsentscheidungen des EGMR wurden die doppelte Ausgabe des „vol.“ bei der Fundstelle „jugdements“ entfernt.

-- Version 2.0.3 --
- Ebd. + Seitenausgabe bei unmittelbarer Wiederholung, aber nicht identische Zitatseite bei Gerichtsentscheidungen hinzugefügt
- Tübinger Kommentar als Sachtitel hinzugefügt

-- Version 2.1.0 --
Bessere Handhabung von Ersetzungen
Bei Urteilen werden nun Zeichenketten wie "Kammer" und "Senat" aus dem Titel beim Zitieren entfernt. Außerdem werden nun "Urteil" und "Beschluss" in der "Art der Entscheidung" durch Komponentenfilter zu "Urt." und "Beschl." abgekürzt. Vorher war diese Ersetzung in den Zitationseigenschaften angelegt, was dazu führte, dass egal wo diese Zeichenketten abgekürzt wurden.

-- Version 2.1.1 --
Paralleltitel nun auch bei Monographien als Diss.
Wenn eine Monographie zugl. eine Diss. ist und das Werk einen Paralleltitel enthält, dann wird das zit. als auch ans Ende des LV-Eintrags gesetzt

-- Version 2.1.2 --
Namensabkürzung bei Uneindeutigkeit hinzugefügt
Wenn mehr als zwei zitierte Autoren denselben Nachnamen haben, dann wird der Vorname abgekürzt davor ausgegeben, sodass die Ungenauigkeit vermieden wird.

-- Version 2.2.0 --
- einheitliche Abkürzung "u. a." statt "et al."
- neue Komponentenprogrammierung, um den Titel ausgeben zu lassen, falls Paralleltitel oder gewöhnliche Abkürzung leer ist.
- "Loseblatt" wurde in einem Fall ausgegeben, wo er nicht hätte ausgegeben werden dürfen (Gesetzeskommentar)
- ein paar Verbesserungen hinsichtlich Benennung von Vorlagen und Komponenten

-- Version 2.2.1 --
- Abkürzungen der Autoren in Fußnoten nun einheitlich: Ab dem dritten Autor/Herausgeber/Begründer wird nur der erste genannt und dann u. a. angefügt. Dies wurde parallel auch in den Zitierangaben im LV umgesetzt.
- Absturzproblem gelöst durch neue Komponentenprogrammierung, bei dem der Paralleltitel oder die übl. Abkürzung durch den Titel ersetzt wird, wenn eins der beiden leer ist (nur in den Zitierangaben am Ende eines LV-Eintrags).
- überflüssige Vorlage wegen Titel in englischer Sprache in Hochschulschrift gelöscht.
- Untertitel und Titelzusätze werden nun durch einen Punkt vom Titel getrennt.
- Bei Gerichtsurteilen wird nun auch "Große Kammer" oder "Großer Senat" vom Gerichtsnamen bei der Ausgabe bereinigt.


> [!TIP]
> Für die detaillierte Anleitung zur Benutzung meines Zitierstils [besuche meinen Blog](https://blogs.urz.uni-halle.de/simpletricks/2022/10/der-citavi-stil-fuer-jura/).

Neben den selbst erstellten Codes für Komponenten und Vorlagen möchte ich auch auf das offizielle Repo von Citavi aufmerksam machen, in dem auch sehr viele nütztliche Codes zu finden sind: https://github.com/LUMIVERO/C6-Citation-Style-Scripts

Unterstütze meinen freien Content:

<a href="https://www.buymeacoffee.com/justanotherjurastudent" target="_blank"><img src="https://cdn.buymeacoffee.com/buttons/v2/default-yellow.png" alt="Buy Me A Coffee" style="height: 60px !important;width: 217px !important;" ></a>
