<template>
  <div :class="['app-wrapper', currentTheme]">
    
    <div class="cyber-background">
      <div class="grid-overlay"></div>
      <div class="glow-orb orb-1"></div>
      <div class="glow-orb orb-2"></div>
    </div>

    <header class="top-nav">
      <div class="logo">
        <i class="fas fa-cubes"></i>
        <h1>TreeBench <span>Enterprise</span></h1>
      </div>
      
      <div class="nav-controls">
        <div class="control-group">
          <button :class="{ active: lang === 'en' }" @click="lang = 'en'">EN</button>
          <button :class="{ active: lang === 'de' }" @click="lang = 'de'">DE</button>
        </div>
        <div class="control-group theme-toggle">
          <button @click="toggleTheme">
            <i :class="currentTheme === 'light-mode' ? 'fas fa-moon' : 'fas fa-sun'"></i>
            {{ t.themeToggle }}
          </button>
        </div>
      </div>
    </header>

    <main class="dashboard-layout">
      
      <aside class="sidebar">
        <div class="panel-card">
          <h3><i class="fas fa-sliders-h"></i> {{ t.controlPanel }}</h3>
          <p class="panel-desc">{{ t.panelDesc }}</p>
          
          <button class="btn-primary" @click="runBenchmark" :disabled="isLoading">
            <i class="fas fa-play" v-if="!isLoading"></i>
            <i class="fas fa-spinner fa-spin" v-else></i>
            {{ isLoading ? t.processing : t.startTest }}
          </button>
          
          <div class="error-box" v-if="errorMsg">
            <i class="fas fa-exclamation-triangle"></i> {{ errorMsg }}
          </div>
        </div>

        <div class="panel-card mt-4">
          <h3><i class="fas fa-chart-pie"></i> {{ t.modelSelect }}</h3>
          <select v-model="chartType" class="form-select mb-2">
            <option value="bar">{{ t.chartBar }}</option>
            <option value="line">{{ t.chartLine }}</option>
            <option value="radar">{{ t.chartRadar }}</option>
          </select>

          <h3 class="mt-4"><i class="fas fa-filter"></i> {{ t.metricSelect }}</h3>
          <select v-model="selectedMetric" class="form-select">
            <option value="time">{{ t.metricTime }} (Insert, Search, Delete)</option>
            <option value="steps">{{ t.metricSteps }}</option>
            <option value="rotations">{{ t.metricRotations }}</option>
            <option value="depth">{{ t.metricDepth }} (Max/Min Depth)</option>
            <option value="ram">{{ t.metricRam }}</option>
          </select>
        </div>
      </aside>

      <section class="workspace">
        <div class="stats-grid">
          <div class="stat-card">
            <div class="stat-icon"><i class="fas fa-database"></i></div>
            <div class="stat-info">
              <span>{{ t.statNodes }}</span>
              <h4>100,000</h4>
            </div>
          </div>
          <div class="stat-card">
            <div class="stat-icon"><i class="fas fa-server"></i></div>
            <div class="stat-info">
              <span>{{ t.statStatus }}</span>
              <h4 :class="isLoading ? 'text-warning' : 'text-success'">
                {{ isLoading ? t.statusProcessing : t.statusConnected }}
              </h4>
            </div>
          </div>
        </div>

        <div class="panel-card chart-container">
          <h3>{{ t.chartTitle }}</h3>
          <div v-if="!benchmarkResults.length && !isLoading" class="empty-state">
            <i class="fas fa-chart-bar"></i>
            <p>{{ t.emptyData }}</p>
          </div>
          <apexchart 
            v-if="benchmarkResults.length && !isLoading"
            width="100%" 
            height="400" 
            :type="chartType" 
            :options="chartOptions" 
            :series="chartSeries">
          </apexchart>
        </div>
      </section>
    </main>

    <div class="ai-widget">
      <div class="ai-chat-window" v-if="isAiOpen">
        <div class="ai-header">
          <span><i class="fas fa-robot"></i> Ege-AI Enterprise</span>
          <button @click="isAiOpen = false"><i class="fas fa-times"></i></button>
        </div>
        <div class="ai-body" ref="chatBody">
          <div v-for="(msg, index) in chatMessages" :key="index" :class="['ai-msg', msg.sender]">
            {{ msg.text }}
          </div>
          <div class="ai-msg bot typing" v-if="isAiTyping">
            <span>.</span><span>.</span><span>.</span>
          </div>
        </div>
        <div class="ai-input">
          <input type="text" v-model="userQuery" @keyup.enter="sendAiMessage" :placeholder="t.aiPlaceholder">
          <button @click="sendAiMessage"><i class="fas fa-paper-plane"></i></button>
        </div>
      </div>
      <button class="ai-trigger pulse-effect" @click="isAiOpen = !isAiOpen">
        <i class="fas fa-robot"></i>
      </button>
    </div>

  </div>
</template>

<script setup>
import { ref, computed, nextTick, watch } from 'vue';

const lang = ref('en');
const currentTheme = ref('light-mode');
const chartType = ref('bar');
const selectedMetric = ref('time'); 
const isLoading = ref(false);
const isAiOpen = ref(false);
const benchmarkResults = ref([]); 
const errorMsg = ref('');

const userQuery = ref('');
const isAiTyping = ref(false);
const chatBody = ref(null);
const chatMessages = ref([
  { sender: 'bot', text: 'Hello! I am ready to analyze system metrics.' }
]);

const dictionary = {
  en: {
    themeToggle: 'Toggle Mode',
    controlPanel: 'System Control',
    panelDesc: 'Test tree structures on a dataset of 100,000 nodes.',
    startTest: 'Run Benchmark',
    processing: 'Calculating...',
    modelSelect: 'Chart Model',
    metricSelect: 'Data Metric',
    metricTime: 'Time (ms)',
    metricSteps: 'Total Traversals (Steps)',
    metricDepth: 'Tree Depth',
    metricRotations: 'Total Rotations',
    metricRam: 'RAM Usage (KB)',
    chartBar: 'Bar Chart',
    chartLine: 'Line Chart',
    chartRadar: 'Radar Chart',
    statNodes: 'Nodes Processed',
    statStatus: 'API Status',
    statusConnected: 'Connected',
    statusProcessing: 'Processing',
    chartTitle: 'Performance Analytics',
    emptyData: 'Run the benchmark to render charts.',
    aiPlaceholder: 'Type a message...',
    aiWelcome: 'Hello! I am ready to analyze system metrics.'
  },
  de: {
    themeToggle: 'Modus Ändern',
    controlPanel: 'Systemsteuerung',
    panelDesc: 'Testen Sie Baumstrukturen mit 100.000 Knoten.',
    startTest: 'Benchmark Starten',
    processing: 'Berechne...',
    modelSelect: 'Diagrammmodell',
    metricSelect: 'Datenmetrik',
    metricTime: 'Zeit (ms)',
    metricSteps: 'Gesamtschritte (Traversals)',
    metricDepth: 'Baumtiefe',
    metricRotations: 'Gesamtrotationen',
    metricRam: 'RAM-Nutzung (KB)',
    chartBar: 'Balkendiagramm',
    chartLine: 'Liniendiagramm',
    chartRadar: 'Radardiagramm',
    statNodes: 'Verarbeitete Knoten',
    statStatus: 'API-Status',
    statusConnected: 'Verbunden',
    statusProcessing: 'Verarbeitung',
    chartTitle: 'Leistungsanalyse',
    emptyData: 'Starten Sie den Benchmark.',
    aiPlaceholder: 'Nachricht...',
    aiWelcome: 'Hallo! Ich bin bereit, Systemmetriken zu analysieren.'
  }
};

const t = computed(() => dictionary[lang.value]);

watch(lang, () => {
  if (chatMessages.value.length > 0 && chatMessages.value[0].sender === 'bot') {
    chatMessages.value[0].text = t.value.aiWelcome;
  }
});

const sendAiMessage = async () => {
  if (!userQuery.value.trim()) return;
  const text = userQuery.value;
  chatMessages.value.push({ sender: 'user', text });
  userQuery.value = '';
  await nextTick();
  if (chatBody.value) chatBody.value.scrollTop = chatBody.value.scrollHeight;
  isAiTyping.value = true;
  
  setTimeout(() => {
    isAiTyping.value = false;
    let reply = lang.value === 'de' ? "Metriken analysiert." : "Metrics analyzed.";
    chatMessages.value.push({ sender: 'bot', text: reply });
    nextTick(() => { if (chatBody.value) chatBody.value.scrollTop = chatBody.value.scrollHeight; });
  }, 1000);
};

const toggleTheme = () => {
  currentTheme.value = currentTheme.value === 'light-mode' ? 'dark-mode' : 'light-mode';
};

// --- APEXCHARTS YAPILANDIRMASI ---
const chartOptions = computed(() => {
  const isDark = currentTheme.value === 'dark-mode';
  
  const getIconForTree = (name) => {
    if (!name) return '🌳 Unknown';
    const n = name.toLowerCase();
    if (n.includes('splay')) return '🔄 ' + name;
    if (n.includes('avl')) return '⚖️ ' + name;
    if (n.includes('red') || n.includes('black')) return '🔴⚫ ' + name;
    if (n.includes('b-tree') || n.includes('bplus') || n.includes('b+')) return '🗄️ ' + name;
    if (n.includes('quad')) return '🗺️ ' + name;
    return '🌳 ' + name;
  };

  const categories = benchmarkResults.value.length 
    ? benchmarkResults.value.map(r => getIconForTree(r.treeName)) 
    : [];

  return {
    chart: { background: 'transparent', foreColor: isDark ? '#e0e0e0' : '#333', toolbar: { show: false } },
    theme: { mode: isDark ? 'dark' : 'light' },
    xaxis: { 
      categories: categories,
      labels: { style: { fontSize: '13px', fontWeight: 600 } }
    },
    colors: ['#0f62fe', '#27c93f', '#ffbd2e', '#ff5f56', '#a855f7'],
    stroke: { curve: 'smooth', width: 3 },
    dataLabels: { 
      enabled: true,
      style: { fontSize: '12px', fontWeight: 'bold', cssClass: 'blinking-datalabel' },
      background: {
        enabled: true,
        foreColor: isDark ? '#fff' : '#000',
        padding: 4,
        borderRadius: 4,
        borderWidth: 0,
        opacity: 0.8,
      },
      formatter: function (val) {
        if (val > 10000) return (val / 1000).toFixed(1) + 'k';
        return parseFloat(val).toFixed(1);
      }
    }
  };
});

const chartSeries = computed(() => {
  if (!benchmarkResults.value.length) return [];
  const data = benchmarkResults.value;

  switch (selectedMetric.value) {
    case 'time':
      return [
        { name: 'Insert (ms)', data: data.map(r => r.insertTimeMs || 0) },
        { name: 'Search (ms)', data: data.map(r => r.searchTimeMs || 0) },
        { name: 'Delete (ms)', data: data.map(r => r.deleteTimeMs || 0) }
      ];
    case 'steps':
      return [
        { name: lang.value === 'de' ? 'Schritte' : 'Steps', data: data.map(r => r.totalSteps || 0) }
      ];
    case 'depth':
      return [
        { name: 'Max Depth', data: data.map(r => r.maxDepth || 0) },
        { name: 'Min Depth', data: data.map(r => r.minDepth || 0) }
      ];
    case 'rotations':
      return [
        { name: 'Total Rotations', data: data.map(r => r.totalRotations || 0) }
      ];
    case 'ram':
      return [
        { name: 'RAM (KB)', data: data.map(r => r.ramCostKb || 0) }
      ];
    default:
      return [];
  }
});

const runBenchmark = async () => {
  isLoading.value = true;
  errorMsg.value = '';
  
  try {
    
const response = await fetch('http://localhost:5174/api/Benchmark/run?mode=1', {
      method: 'POST',
      headers: { 'Accept': 'application/json', 'Content-Type': 'application/json' }
    });

    if (!response.ok) throw new Error(`Status: ${response.status}`);
    
    const data = await response.json();
    benchmarkResults.value = Array.isArray(data) ? data : [data];
    
  } catch (err) {
    errorMsg.value = lang.value === 'de' ? "Serverfehler. API prüfen." : "Server error. Check API.";
  } finally {
    isLoading.value = false;
  }
};
</script>

<style>
@import url('https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css');
@import url('https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700&display=swap');

* { margin: 0; padding: 0; box-sizing: border-box; }
body { font-family: 'Inter', sans-serif; transition: all 0.3s ease; }

.light-mode { --bg-color: #f8fafc; --surface: #ffffff; --text-main: #0f172a; --text-muted: #64748b; --border: #e2e8f0; --primary: #0f62fe; }
.dark-mode { --bg-color: #0b0f19; --surface: #131c2e; --text-main: #f8fafc; --text-muted: #94a3b8; --border: #1e293b; --primary: #3b82f6; }

.app-wrapper { background-color: var(--bg-color); color: var(--text-main); min-height: 100vh; display: flex; flex-direction: column; position: relative; overflow-x: hidden; }
.cyber-background { position: fixed; top: 0; left: 0; width: 100vw; height: 100vh; pointer-events: none; z-index: 0; overflow: hidden; }
.grid-overlay { position: absolute; width: 100%; height: 100%; background-image: linear-gradient(var(--border) 1px, transparent 1px), linear-gradient(90deg, var(--border) 1px, transparent 1px); background-size: 40px 40px; opacity: 0.25; }
.glow-orb { position: absolute; border-radius: 50%; filter: blur(80px); opacity: 0.15; animation: float-orb 15s ease-in-out infinite alternate; }
.orb-1 { width: 400px; height: 400px; background: var(--primary); top: -100px; left: -100px; }
.orb-2 { width: 500px; height: 500px; background: #10b981; bottom: -150px; right: -150px; animation-delay: -5s; }
@keyframes float-orb { 0% { transform: translateY(0px) scale(1); } 100% { transform: translateY(30px) scale(1.05); } }

.top-nav { display: flex; justify-content: space-between; align-items: center; padding: 1rem 3rem; background-color: var(--surface); border-bottom: 1px solid var(--border); z-index: 10; box-shadow: 0 4px 20px rgba(0,0,0,0.02); }
.logo { display: flex; align-items: center; gap: 10px; font-size: 1.2rem; color: var(--primary); }
.logo h1 { font-size: 1.2rem; font-weight: 700; color: var(--text-main); }
.logo span { font-weight: 400; color: var(--text-muted); }

.nav-controls { display: flex; gap: 20px; }
.control-group button { background: transparent; border: 1px solid var(--border); color: var(--text-muted); padding: 6px 12px; cursor: pointer; font-family: 'Inter'; font-weight: 600; transition: 0.2s; }
.control-group button:first-child { border-radius: 6px 0 0 6px; }
.control-group button:last-child { border-radius: 0 6px 6px 0; border-left: none; }
.control-group button.active { background: var(--primary); color: white; border-color: var(--primary); }
.theme-toggle button { border-radius: 6px !important; border-left: 1px solid var(--border) !important; }

.dashboard-layout { display: grid; grid-template-columns: 320px 1fr; gap: 2rem; padding: 2rem 3rem; flex-grow: 1; z-index: 5; }
.panel-card { background-color: var(--surface); border: 1px solid var(--border); border-radius: 12px; padding: 1.5rem; box-shadow: 0 10px 30px rgba(0,0,0,0.03); }
.panel-card h3 { font-size: 1.1rem; margin-bottom: 15px; display: flex; align-items: center; gap: 10px; }
.panel-desc { color: var(--text-muted); font-size: 0.95rem; margin-bottom: 20px; line-height: 1.5; }
.mt-4 { margin-top: 1.5rem; }
.mb-2 { margin-bottom: 0.5rem; }
.form-select { width: 100%; padding: 10px; border: 1px solid var(--border); border-radius: 8px; background-color: var(--bg-color); color: var(--text-main); font-family: 'Inter'; outline: none; cursor: pointer; }

.btn-primary { width: 100%; padding: 12px; background-color: var(--primary); color: white; border: none; border-radius: 8px; font-size: 1rem; font-weight: 600; cursor: pointer; transition: 0.2s; display: flex; justify-content: center; gap: 10px; align-items: center; }
.btn-primary:hover:not(:disabled) { opacity: 0.9; transform: translateY(-1px); }
.btn-primary:disabled { opacity: 0.6; cursor: not-allowed; }
.error-box { margin-top: 15px; padding: 10px; background: rgba(255, 95, 86, 0.1); color: #ff5f56; border-radius: 8px; font-size: 0.9rem; }

.stats-grid { display: grid; grid-template-columns: repeat(2, 1fr); gap: 1.5rem; margin-bottom: 1.5rem; }
.stat-card { background-color: var(--surface); border: 1px solid var(--border); border-radius: 12px; padding: 1.5rem; display: flex; align-items: center; gap: 15px; box-shadow: 0 10px 30px rgba(0,0,0,0.03); }
.stat-icon { width: 50px; height: 50px; background: rgba(15, 98, 254, 0.1); color: var(--primary); border-radius: 12px; display: flex; align-items: center; justify-content: center; font-size: 1.5rem; }
.stat-info span { color: var(--text-muted); font-size: 0.85rem; text-transform: uppercase; font-weight: 600; }
.stat-info h4 { font-size: 1.5rem; margin-top: 5px; }
.text-success { color: #10b981; }
.text-warning { color: #f59e0b; }

.chart-container { min-height: 400px; display: flex; flex-direction: column; }
.empty-state { margin: auto; display: flex; flex-direction: column; align-items: center; gap: 15px; color: var(--text-muted); }
.empty-state i { font-size: 3rem; opacity: 0.5; }

.blinking-datalabel { animation: data-pulse 1.5s infinite alternate ease-in-out; }
@keyframes data-pulse { 0% { opacity: 1; } 100% { opacity: 0.3; } }

.ai-widget { position: fixed; bottom: 30px; right: 30px; z-index: 1000; display: flex; flex-direction: column; align-items: flex-end; gap: 15px; }
.ai-trigger { width: 60px; height: 60px; border-radius: 50%; background-color: var(--primary); color: white; border: none; font-size: 1.5rem; cursor: pointer; box-shadow: 0 10px 25px rgba(15, 98, 254, 0.4); transition: 0.3s; }
.ai-trigger:hover { transform: scale(1.05); }
.ai-chat-window { width: 350px; background-color: var(--surface); border: 1px solid var(--border); border-radius: 14px; box-shadow: 0 20px 40px rgba(0,0,0,0.15); overflow: hidden; display: flex; flex-direction: column; }
.ai-header { background-color: var(--primary); color: white; padding: 15px; display: flex; justify-content: space-between; font-weight: 600; align-items: center; }
.ai-header button { background: none; border: none; color: white; cursor: pointer; font-size: 1.2rem; }
.ai-body { padding: 15px; height: 260px; overflow-y: auto; background-color: var(--bg-color); display: flex; flex-direction: column; gap: 10px; }
.ai-msg { padding: 10px 14px; border-radius: 10px; font-size: 0.9rem; line-height: 1.4; max-width: 85%; }
.ai-msg.bot { background: var(--surface); border: 1px solid var(--border); align-self: flex-start; color: var(--text-main); }
.ai-msg.user { background: var(--primary); color: white; align-self: flex-end; }
.ai-msg.typing span { animation: blink 1.4s infinite both; font-weight: bold; font-size: 1.2rem; display: inline-block; }
.ai-msg.typing span:nth-child(2) { animation-delay: .2s; }
.ai-msg.typing span:nth-child(3) { animation-delay: .4s; }
@keyframes blink { 0% { opacity: .2; } 20% { opacity: 1; } 100% { opacity: .2; } }
.ai-input { display: flex; border-top: 1px solid var(--border); background-color: var(--surface); }
.ai-input input { flex-grow: 1; border: none; padding: 12px 15px; background: transparent; color: var(--text-main); outline: none; font-size: 0.9rem; }
.ai-input button { background: transparent; border: none; padding: 0 18px; color: var(--primary); cursor: pointer; font-size: 1rem; }
</style>