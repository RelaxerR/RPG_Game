document.getElementById('start-btn').addEventListener('click', () => {
    const name = document.getElementById('player-name').value;
    if (!name) return alert("Назовите себя!");

    document.getElementById('start-screen').classList.remove('active');
    document.getElementById('game-screen').classList.add('active');

    startGame(name);
});

async function loadLeaderboard() {
    // Здесь позже будет fetch к нашему .NET контроллеру
    const list = document.getElementById('leaderboard-list');
    list.innerHTML = "<li>1. Arthur - 1500 xp</li><li>2. Merlin - 1200 xp</li>";
}

async function startGame(name) {
    const response = await fetch('/api/game/start', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(name)
    });

    const data = await response.json();

    // Выводим данные в лог игры
    const log = document.getElementById('log-window');
    log.innerHTML = `<div><strong>${data.message}</strong></div>`;
    log.innerHTML += `<div>Локация: ${data.quest.title}</div>`;
    log.innerHTML += `<div><em>${data.quest.description}</em></div>`;

    console.log("Статы игрока:", data.player);
}

loadLeaderboard();
