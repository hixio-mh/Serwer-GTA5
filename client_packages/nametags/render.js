let playerSpawnHandler = (player) => {
    player.setHealth(70 + 100);
}
mp.events.add("playerSpawn", playerSpawnHandler);