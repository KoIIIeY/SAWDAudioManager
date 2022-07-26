# SAWDAudioManager

Add the package via the package manager (install via git).

If you have not used the standard localization system yet - create localization settings (edit -> project settings -> localization)

Now either add or select any audio source (component on the gameobject), right click and click Localize.

In addition to the standard localization fields, you will see two fields for login and password and a field for the game ID.

Register on https://audio.sa-wd.ru and create a game.
In the first window after creating the game, take the game ID.

Return to Unity, enter your username and password, click Login, then enter the game ID and click set game ID.

If you already have sound tables filled in, click Create online structure - this will upload your sounds to the site along with translations in the same form as you have already done.

Further on the site, go to Packages for your game (the first language in the list is considered the basic package) - you will see your sounds.

Now call your voice actor/translator or other person who is translating for you and ask them to register.

Add it via email to access (at the bottom of the game page)

The translator clicks on the site in "packages" to create a package based on the base package and simply adds the sounds next to the original.

You, as a developer, go back to any localized audio source in Unity and click Download online structure.

Done - the translations are already in the game.
