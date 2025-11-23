# AI-VR-ME Digital Twin Framework
## Stand-Alone Rehabilitation Performance Simulation and Visualization System

[![Unity](https://img.shields.io/badge/Unity-2021.3%2B-black.svg?style=flat&logo=unity)](https://unity.com/)
[![C#](https://img.shields.io/badge/C%23-8.0-blue.svg?style=flat&logo=c-sharp)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

---

## Overview

The AI-VR-ME Digital Twin Framework is a comprehensive Unity-based system for modeling, simulating, and visualizing rehabilitation-related performance data. This phase operates **independently of hardware** and establishes the analytical and visualization foundation for future integration with immersive rehabilitation environments.

### Key Features

✅ **AI-Driven Data Generation** - Synthetic rehabilitation data with realistic behavioral patterns  
✅ **3D Visualization** - Interactive digital twin avatar with real-time feedback  
✅ **Analytics Dashboard** - Comprehensive metrics display with charts and anomaly detection  
✅ **Model Comparison** - Side-by-side comparison of different simulation models  
✅ **Multiple Export Formats** - JSON, CSV, and analytical reports  
✅ **Interactive Playback** - Play, pause, step-through, and seek controls  
✅ **Extensible Architecture** - Ready for VR hardware integration

---

## Project Objectives

This framework addresses **all five objectives** of the AI-VR-ME phase:

### ✓ Objective 1: Independent Digital Twin Simulation Framework
Modular simulation engine producing synthetic rehabilitation data with AI-driven inference and stochastic methods.

### ✓ Objective 2: Mock Data Generation in Controlled Environment
Data generation pipeline producing structured datasets (JSON/CSV) for Unity visualization.

### ✓ Objective 3: Interactive 3D Visualization and Analytics Dashboard
Virtual environment with humanoid avatar and real-time analytical metrics display.

### ✓ Objective 4: Model Comparison and Interactive Control
Interface elements for switching between models, with pause, replay, and data modification controls.

### ✓ Objective 5: Reporting, Data Exchange, and Evaluation Protocols
Export pipeline logging analytical outcomes into standardized report files.

---

## Quick Start

### Prerequisites
- Unity 2021.3 LTS or newer
- TextMeshPro (included with Unity)
- Basic C# knowledge (optional)

### Installation (5 minutes)

1. **Create Unity Project**
   ```
   Unity Hub → New Project → 3D (Core or URP)
   Project Name: AI-VR-ME-DigitalTwin
   ```

2. **Import Scripts**
   ```
   Copy all scripts to: Assets/Scripts/AIVRDigitalTwin/
   ```

3. **Setup Scene**
   ```
   Create GameObject: "DigitalTwinSystem"
   Add Component: DigitalTwinManager
   Create child objects: DataGenerator, Avatar, SimulationController, DataExporter
   Link references in Inspector
   ```

4. **Press Play!**
   ```
   System auto-generates sample session
   Check Console for confirmation
   Find exported files in Application.persistentDataPath/ExportedData/
   ```

---

## Core Components

### 1. Data Layer
- **SyntheticDataGenerator**: AI-driven data generation with behavioral patterns
- **RehabilitationDataModel**: Core data structures (sessions, frames, metrics)
- **DataExporter**: Multi-format export (JSON, CSV, reports)

### 2. Visualization Layer
- **DigitalTwinAvatar**: 3D humanoid representation with motion and color feedback
- **AnalyticsDashboard**: Real-time metrics, charts, and anomaly tracking
- **LineChartRenderer**: Custom real-time chart visualization

### 3. Control Layer
- **SimulationController**: Playback orchestration and model comparison
- **DigitalTwinManager**: Top-level system coordination and UI management

---

## Usage Examples

### Basic Session Generation
```csharp
// Automatic on startup (default)
// Or manually:
digitalTwinManager.GenerateSampleSession();
```

### Playback Control
```csharp
simulationController.Play();      // Start playback
simulationController.Pause();     // Pause
simulationController.Stop();      // Stop and reset
simulationController.StepForward(); // Frame-by-frame
simulationController.SeekToTime(120.0f); // Jump to 2 minutes
```

### Model Comparison
```csharp
// Switch between models
simulationController.SwitchToModel(0); // Model A
simulationController.SwitchToModel(1); // Model B

// Generate comparison dataset
simulationController.GenerateModelComparison(5); // 5 sessions per model
```

### Data Export
```csharp
dataExporter.ExportSessionToJson(session);
dataExporter.ExportSessionToCsv(session);
dataExporter.ExportMetricsSummary(session);
dataExporter.ExportAnalyticalReport(session);
```

---

## Data Output

### Generated Data Includes:

**Performance Metrics:**
- Session accuracy (0-1)
- Fatigue indicators (0-1)
- Prediction confidence (0-1)
- Performance deviation (standard deviations)

**Analysis Results:**
- Anomaly detection (boolean + timestamps)
- Session-level aggregates
- Trend analysis
- Confidence intervals

### Export Formats:

**JSON:** Complete structured data
```json
{
  "sessionId": "abc123...",
  "frames": [{ "timeStamp": 0.0, ... }],
  "metrics": { "averageAccuracy": 0.823, ... }
}
```

**CSV:** Tabular format (9000+ rows for 5-min session)
```csv
SessionId,TimeStamp,LeftArmPosX,LeftArmPosY,...
abc123,0.000,0.300,1.200,...
```

**Analytical Report:** Human-readable summary with insights
```
=== REHABILITATION SESSION ANALYTICAL REPORT ===
Average Accuracy: 82.3%
Peak Fatigue: 68.5%
Total Anomalies: 47
[Detailed analysis...]
```

---

## Key Features Detail

### AI-Driven Simulation
- Realistic fatigue models
- Accuracy degradation over time
- Stochastic noise injection
- Anomaly generation
- Behavioral pattern learning

### Real-time Visualization
- Color-coded performance states (green/yellow/red)
- Motion trails for arm tracking
- Smooth interpolation
- Visual anomaly alerts
- Configurable visual feedback

### Analytics Dashboard
- Live metrics display
- Historical trend charts
- Anomaly timeline
- Confidence indicators
- Status alerts

### Interactive Controls
- Play/Pause/Stop
- Frame-by-frame stepping
- Time seeking
- Speed adjustment (0.1x to 5x)
- Loop playback

### Model Comparison
- Multiple model configurations
- Batch session generation
- Comparative analysis
- Parameter variation
- Statistical summaries

---

## Performance

**Typical Performance on Standard Hardware:**

| Operation | Speed |
|-----------|-------|
| Generate 5-min session | ~1 second |
| Real-time playback (30 FPS) | Smooth, 60 FPS render |
| Export all formats | <5 seconds |
| Batch generation (10 sessions) | ~10 seconds |

**Memory Usage:**
- Single session: ~2 MB
- 10 sessions: ~20 MB
- Full system (active): ~35 MB

---

## Extension and Customization

### Adding Custom Metrics
```csharp
// 1. Add to PerformanceFrame
public float myMetric;

// 2. Calculate in generator
frame.myMetric = CalculateCustomMetric(frame);

// 3. Display in dashboard
myMetricText.text = $"My Metric: {frame.myMetric:F2}";
```

### Custom Behavioral Models
```csharp
ModelParameters customModel = new ModelParameters("Custom");
customModel.learningRate = 0.15f;
customModel.fatigueRate = 0.008f;
// Add to available models list
```

## Technical Requirements

**Minimum:**
- Unity 2021.3 LTS
- 4 GB RAM
- Integrated graphics

**Recommended:**
- Unity 2022.3 LTS or newer
- 8 GB RAM
- Dedicated GPU
- SSD storage
