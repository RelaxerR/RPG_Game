document.getElementById('start-btn').addEventListener('click', async () => {
    const nameInput = document.getElementById('player-name');
    if (!nameInput.value) return alert("Введите имя!");

    currentPlayerName = nameInput.value;
    document.querySelector('.sidebar').classList.add('hidden');

    const response = await fetch('/api/game/start', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(currentPlayerName)
    });

    if (response.ok) {
        const data = await response.json();
        document.getElementById('start-screen').style.display = 'none';
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

// Текущее имя игрока (сохраняем при старте)
let currentPlayerName = "";

function updatePlayerStats(player) {
    if (!player) return;
    // Используем проверку на оба регистра (на всякий случай)
    const hp = player.currentHealth ?? player.CurrentHealth;
    const maxHp = player.maxHealth ?? player.MaxHealth;
    const lvl = player.level ?? player.Level;
    const gold = player.gold ?? player.Gold;
    const xp = player.experience ?? player.Experience;

    document.getElementById('stat-hp').innerText = `${hp}/${maxHp}`;
    document.getElementById('stat-lvl').innerText = lvl;
    document.getElementById('stat-gold').innerText = gold;
    document.getElementById('stat-xp').innerText = `${xp}/${lvl * 100}`;
}

function renderGameState(data) {
    console.log("Response data:", data);
    const log = document.getElementById('log-window');
    const actions = document.getElementById('actions-panel');

    updatePlayerStats(data.player ?? data.Player);

    // 1. ФОРМИРУЕМ HTML СОБЫТИЙ
    let eventsHtml = "";
    let hasLevelUp = false;

    const eventsList = data.events || data.Events;
    if (eventsList && eventsList.length > 0) {
        eventsHtml = eventsList.map(ev => {
            const msg = ev.message || ev.Message;
            const type = (ev.type || ev.Type || "info").toLowerCase();

            if (type === "levelup") {
                hasLevelUp = true;
            }

            return `<div class="event-inline msg-${type}" style="display:block; margin-bottom:5px;">[ ${msg} ]</div>`;
        }).join("");
    }

    // 2. ВЫВОДИМ В ЛОГ
    const quest = data.quest || data.Quest;
    const title = quest.title || quest.Title;
    const desc = data.text || quest.description || quest.Description;

    // Проверяем, есть ли уже записи в логе (не первый квест)
    const hasPreviousEntries = log.children.length > 0;

    // Разделитель ТОЛЬКО между квестами (не перед первым)
    const separatorHtml = hasPreviousEntries
        ? `<hr class="quest-separator ${hasLevelUp ? 'levelup-separator' : ''}">`
        : "";

    // Убрали внутренний hr из квеста
    const entryHtml = `
        <div class="quest-entry">
            ${separatorHtml}
            ${eventsHtml}
            <p><strong>${title}</strong></p>
            <p>${desc}</p>
        </div>
    `;

    log.insertAdjacentHTML('beforeend', entryHtml);

    // Кнопки
    actions.innerHTML = "";
    const choices = quest.toIds || quest.ToIds;
    const currentId = quest.id;

    if (choices) {
        for (const [id, label] of Object.entries(choices)) {
            const btn = document.createElement('button');
            btn.className = 'retro-btn action-btn';
            btn.innerText = label;
            btn.onclick = () => sendAction(parseInt(currentId), parseInt(id));
            actions.appendChild(btn);
        }
    }

    log.scrollTop = log.scrollHeight;
}

async function sendAction(currentId, targetId) {
    const response = await fetch('/api/game/action', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
            playerName: currentPlayerName,
            currentQuestId: currentId,
            targetQuestId: targetId
        })
    });
    if (response.ok) {
        const data = await response.json();
        renderGameState(data);
    }
}

loadLeaderboard();
