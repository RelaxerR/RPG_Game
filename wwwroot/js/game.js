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

let currentPlayerName = "";

function updatePlayerStats(player) {
    if (!player) return;
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

    // 1. СОБЫТИЯ
    let eventsHtml = "";
    let hasLevelUp = false;
    const eventsList = data.events || data.Events;
    if (eventsList && eventsList.length > 0) {
        eventsHtml = eventsList.map(ev => {
            const msg = ev.message || ev.Message;
            const type = (ev.type || ev.Type || "info").toLowerCase();
            if (type === "levelup") hasLevelUp = true;
            return `<div class="event-inline msg-${type}">[ ${msg} ]</div>`;
        }).join("");
    }

    // 2. ЛОГ
    const quest = data.quest || data.Quest;
    const title = quest.title || quest.Title;
    const desc = data.text || quest.description || quest.Description;

    const hasPreviousEntries = log.children.length > 0;
    const separatorHtml = hasPreviousEntries
        ? `<hr class="quest-separator ${hasLevelUp ? 'levelup-separator' : ''}">`
        : "";

    const entryHtml = `
    <div class="quest-entry">
    ${separatorHtml}
    ${eventsHtml}
    <p><strong>${title}</strong></p>
    <p>${desc}</p>
    </div>
    `;
    log.insertAdjacentHTML('beforeend', entryHtml);

    // 3. КНОПКИ
    actions.innerHTML = "";
    const combatState = data.combatState || data.CombatState;

    if (combatState?.isActive) {
        renderCombatButtons(actions, quest.id, combatState);
    } else {
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
    }
    log.scrollTop = log.scrollHeight;
}

function renderCombatButtons(container, questId, combatState) {
    const attackBtn = document.createElement('button');
    attackBtn.className = 'retro-btn action-btn';
    attackBtn.innerText = `⚔️ АТАКОВАТЬ (Враг HP: ${combatState.enemyCurrentHealth}/${combatState.enemyMaxHealth})`;
    attackBtn.style.borderColor = '#ff3333';
    attackBtn.onclick = () => sendCombatAction(questId, -1);
    container.appendChild(attackBtn);

    const fleeBtn = document.createElement('button');
    fleeBtn.className = 'retro-btn action-btn';
    fleeBtn.innerText = '🏃 СБЕЖАТЬ (-5-15 HP, частичный XP)';
    fleeBtn.style.borderColor = '#ffaa00';
    fleeBtn.onclick = () => sendCombatAction(questId, -2);
    container.appendChild(fleeBtn);

    const negotiateBtn = document.createElement('button');
    negotiateBtn.className = 'retro-btn action-btn';
    negotiateBtn.innerText = '💬 ДОГОВОРИТЬСЯ (40% шанс)';
    negotiateBtn.style.borderColor = '#33aaff';
    negotiateBtn.onclick = () => sendCombatAction(questId, -3);
    container.appendChild(negotiateBtn);
}

async function sendCombatAction(currentId, actionType) {
    const response = await fetch('/api/game/combat', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
            playerName: currentPlayerName,
            currentQuestId: currentId,
            actionType: actionType
        })
    });

    if (response.ok) {
        const data = await response.json();
        renderGameState(data);
    } else if (response.status === 400) {
        document.getElementById('game-screen').classList.remove('active');
        document.getElementById('start-screen').style.display = 'block';
        document.querySelector('.sidebar').classList.remove('hidden');
        alert("Ваш герой погиб! Начните заново.");
    }
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