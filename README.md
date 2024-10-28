# The Game of Sisyphus - Unity ML-Agents

[![Sisyphus Game]](https://www.youtube.com/watch?v=vLOHMt0x4Sk)


This project replicates "The Game of Sisyphus" using Unity's ML-Agents framework.  
In this environment, an agent learns to push a ball up a hill while avoiding obstacles, mimicking the myth of Sisyphus.  
The agent's objective is to reach the top of the hill without letting the ball roll back down.

## Table of Contents

- [Project Overview](#project-overview)
- [Version](#version)
- [Running the Simulation](#running the Simulation)
- [License](#license)

## Project Overview

In this project, an agent is tasked with pushing a ball up a hill, facing physical challenges like rolling resistance and slope.  
The agent is trained using **Reinforcement Learning (RL)**, specifically with the **Proximal Policy Optimization (PPO)** algorithm provided by Unity ML-Agents.  
The goal of this project is to create an immersive RL environment where the agent can efficiently learn to complete this task.

## Version

Unity version: 2021.3.37f1
ML Agents: 2.0.0
ML Agents Extensions: 0.4.0

We cannot guarantee compatibility with other versions, and you may need to downgrade `numpy` if required.

## Running the Simulation

Once you have opened the project in Unity, follow these steps to run the simulation:

1. Open the **EnvScene** scene in Unity.
2. Click **Play** to start the environment simulation.
3. You can replace Prefab **AgentPrefab** of **Environment Spawnwer** in **Main Camera**(or Model of Each Prefab).
4. Watch the agent interact with the ball and attempt to push it up the hill.
5. You can play heuristic mode by removing model in agent.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for more details.
