let currentPlayerName = "";
const CIRCUMFERENCE = 220;

// === СТАРТ ИГРЫ ===
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

// === ОТРИСОВКА СОСТОЯНИЯ ИГРЫ ===
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

    // 1. Проверка наличия заголовка сцены
    const quest = data.quest ?? data.Quest;
    const title = quest ? quest.title ?? quest.Title : "Неизвестная локация";

    // 2. СОБЫТИЯ
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

    // 3. ЛОГ
    const desc = data.text || quest?.description || quest?.Description || "Нет описания";
    const hasPreviousEntries = log.children.length > 0;
    const separatorHtml = hasPreviousEntries
        ? `<hr class="quest-separator ${hasLevelUp ? 'levelup-separator' : ''}">`
        : "";

    // ← ГЕНЕРИРУЕМ УНИКАЛЬНЫЙ ID ЗАРАНЕЕ
    const containerId = `typing-${Date.now()}`;
    const entryHtml = `
        <div class="quest-entry">
            ${separatorHtml}
            ${eventsHtml}
            <p><strong>${title}</strong></p>
            <div class="typing-container" id="${containerId}"></div>
        </div>
    `;
    log.insertAdjacentHTML('beforeend', entryHtml);

    // 4. ОСТАНОВИМ СТАРЫЙ ЭФФЕКТ ПЕЧАТАНИЯ
    const existingTypingContainers = log.querySelectorAll('.typing-text');
    existingTypingContainers.forEach(container => {
        container.classList.remove('typing-text');
    });

    // 5. НОВЫЙ ЭФФЕКТ ПЕЧАТАНИЯ
    // ← ИСПРАВЛЕНИЕ: используем getElementById вместо querySelector
    const typingContainer = document.getElementById(containerId);

    // ← Блокируем панель действий пока идёт анимация
    actions.classList.add('disabled');

    if (typingContainer && desc) {
        // ← Передаём callback, который выполнится ПОСЛЕ завершения печати
        typeWriterEffect(typingContainer, desc, 20, () => {
            // ← Только после завершения текста: рендерим и показываем кнопки
            renderButtons(actions, data, quest);
            actions.classList.remove('disabled');
            log.scrollTop = log.scrollHeight;
        });
    } else {
        renderButtons(actions, data, quest);
        actions.classList.remove('disabled');
    }

    log.scrollTop = log.scrollHeight;
}

// === ЭФФЕКТ ПЕЧАТНОЙ МАШИНКИ (с поддержкой callback) ===
function typeWriterEffect(element, text, speed = 30, onComplete = null) {
    let i = 0;
    element.classList.add('typing-text');
    element.textContent = ""; // ← Очищаем перед началом

    let interval = setInterval(() => {
        if (i < text.length) {
            element.textContent += text.charAt(i);
            i++;
        } else {
            clearInterval(interval);
            element.classList.remove('typing-text');
            if (onComplete) onComplete(); // ← Вызываем колбэк по завершении
            return;
        }
        const log = document.getElementById('log-window');
        log.scrollTop = log.scrollHeight;
    }, speed);
}

// === ОТДЕЛЬНЫЙ РЕНДЕР КНОПОК (с каскадной анимацией) ===
function renderButtons(container, data, quest) {
    container.innerHTML = ""; // ← Очищаем старые кнопки

    const combatState = data.combatState || data.CombatState;
    console.log("Combat state:", combatState);

    if (combatState?.isActive) {
        renderCombatButtons(container, quest?.id, combatState);
    } else {
        const choices = quest?.toIds || quest?.ToIds;
        const currentId = quest?.id;

        if (choices && Object.keys(choices).length > 0) {
            let index = 0;
            for (const [id, label] of Object.entries(choices)) {
                const btn = document.createElement('button');
                btn.className = 'retro-btn action-btn';
                btn.innerText = label;
                // ← Начальное состояние для анимации появления
                btn.style.opacity = '0';
                btn.style.transform = 'translateY(5px)';
                btn.onclick = () => sendAction(parseInt(currentId), parseInt(id));
                container.appendChild(btn);

                // ← Каскадное появление с задержкой
                setTimeout(() => {
                    btn.style.transition = 'opacity 0.3s ease-in, transform 0.3s ease-in';
                    btn.style.opacity = '1';
                    btn.style.transform = 'translateY(0)';
                }, index * 100);
                index++;
            }
        }
    }
}

// === КНОПКИ БОЯ ===
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

// === ОТПРАВКА ДЕЙСТВИЙ ===
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