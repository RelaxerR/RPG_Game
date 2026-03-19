document.getElementById('start-btn').addEventListener('click', async () => {
    const nameInput = document.getElementById('player-name');
    if (!nameInput.value) return alert("Введите имя!");

    currentPlayerName = nameInput.value;

    // Скрываем таблицу лидеров при начале игры
    document.querySelector('.sidebar').classList.add('hidden');
    // Можно также расширить игровое поле на весь экран
    document.querySelector('.game-area').style.flex = "1";

    const response = await fetch('/api/game/start', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(currentPlayerName)
    });

    if (response.ok) {
        const data = await response.json();
        document.getElementById('start-screen').classList.add('hidden');
        document.getElementById('game-screen').classList.add('active');
        renderGameState(data);
    }
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

function renderQuest(data) {
    const log = document.getElementById('log-window');
    const actions = document.getElementById('actions-panel');

    log.innerHTML += `<div class="quest-node">
        <h3>${data.quest.title}</h3>
        <p>${data.text}</p>
    </div>`;

    actions.innerHTML = ""; // Очищаем старые кнопки

    // Создаем кнопки для каждого доступного перехода
    for (const [id, label] of Object.entries(data.quest.toIds)) {
        const btn = document.createElement('button');
        btn.className = 'retro-btn';
        btn.innerText = label;
        btn.onclick = () => sendAction(id); // Функция отправки ID на сервер
        actions.appendChild(btn);
    }
}

// Текущее имя игрока (сохраняем при старте)
let currentPlayerName = "";

function renderGameState(data) {
    const log = document.getElementById('log-window');
    const actions = document.getElementById('actions-panel');

    // 1. Сначала выводим системные уведомления (награды)
    if (data.events && data.events.length > 0) {
        data.events.forEach(ev => {
            const evDiv = document.createElement('div');
            evDiv.className = `system-msg msg-${ev.type.toLowerCase()}`;
            evDiv.innerText = `[${ev.message}]`;
            log.appendChild(evDiv);
        });
    }

    // 2. Затем основной текст квеста
    log.innerHTML += `
        <div class="quest-entry">
            <h3>${data.quest.title}</h3>
            <p>${data.text || data.quest.description}</p>
        </div>
    `;

    // Автопрокрутка лога вниз
    log.scrollTop = log.scrollHeight;

    // 2. Очищаем старые кнопки и создаем новые
    actions.innerHTML = "";

    // Проходим по словарю ToIds (ID квеста : Текст на кнопке)
    for (const [id, label] of Object.entries(data.quest.toIds)) {
        const btn = document.createElement('button');
        btn.className = 'retro-btn action-btn';
        btn.innerText = label;
        btn.onclick = () => sendAction(parseInt(id));
        actions.appendChild(btn);
    }
}

async function sendAction(targetId) {
    const response = await fetch('/api/game/action', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
            playerName: currentPlayerName,
            targetQuestId: targetId
        })
    });

    if (response.ok) {
        const data = await response.json();
        renderGameState(data);
    }
}

loadLeaderboard();
