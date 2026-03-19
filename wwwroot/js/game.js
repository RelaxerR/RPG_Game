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

function startGame(name) {
    console.log(`Игра началась для ${name}`);
    // Инициализация сессии на сервере
}

loadLeaderboard();
