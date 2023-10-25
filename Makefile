build-and-copy: build copy



build:
	dotnet publish

copy:
	rm SharpTournament/bin/Debug/net7.0/publish/CounterStrikeSharp.API.dll
	cp -rf SharpTournament/bin/Debug/net7.0/publish/* /home/volza/temp/cs2/cs2-data/game/csgo/addons/counterstrikesharp/plugins/SharpTournament

start-ds:
	cd /home/volza/temp/cs2
	docker run --rm -it --net=host -v /home/volza/temp/cs2/cs2-data:/home/steam/cs2-dedicated/ --name=cs2-dedicated -e STEAMUSER=cs2_lan2play -e STEAMPASS=vZ23qYxdyFLKZyI1rnbrFHEmGfET -e CS2_BOT_QUOTA=0 lan2play/cs2

fix-mm:
	sed -i '/^			Game	csgo$/i			Game	csgo/addons/metamod' /home/volza/temp/cs2/cs2-data/game/csgo/gameinfo.gi