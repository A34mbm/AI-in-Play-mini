using System;
using System.Collections.Generic;
using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [System.Serializable]
    public class Edge
    {
        public int id;
        public string name;
        public int fromNode;
        public int toNode;
        public int score;
    }

    [System.Serializable]
    public class Node
    {
        public int id;
        public string name;
        public List<int> edgeIds = new List<int>();
    }

    [Header("現在のマップデータ（自動生成されます）")]
    public List<Node> nodes = new List<Node>();
    public List<Edge> edges = new List<Edge>();

    [Header("★チェンジ（入れ替え）テスト用")]
    [Tooltip("入れ替えたい道のID（1つ目）")] public int swapEdgeId1 = 3;
    [Tooltip("入れ替えたい道のID（2つ目）")] public int swapEdgeId2 = 6;

    private void Start()
    {
        InitMap();
    }

    // 他の人が作ったデータを読み込むイメージの初期化処理
    private void InitMap()
    {
        nodes.Clear();
        edges.Clear();

        // マスの自動作成 (A=0, B=1, C=2 ... I=8)
        string[] nodeNames = { "A", "B", "C", "D", "E", "F", "G", "H", "I" };
        for (int i = 0; i < nodeNames.Length; i++)
        {
            CreateNode(nodeNames[i]);
        }

        // 画像に基づいた初期の道データ {出発マスID, 到着マスID, スコア}
        int[,] rawEdgeData = {
            {0, 1, 2},   // ID 0: A ─> B
            {0, 2, 7},   // ID 1: A ─> C
            {1, 3, 8},   // ID 2: B ─> D
            {1, 4, 1},   // ID 3: B ─> E
            {2, 4, 2},   // ID 4: C ─> E
            {2, 5, 3},   // ID 5: C ─> F
            {3, 6, 15},  // ID 6: D ─> G
            {4, 6, 10},  // ID 7: E ─> G
            {4, 7, 12},  // ID 8: E ─> H
            {5, 7, 3},   // ID 9: F ─> H
            {6, 8, 1},   // ID 10: G ─> I (ゴール)
            {7, 8, 2}    // ID 11: H ─> I (ゴール)
        };

        for (int i = 0; i < rawEdgeData.GetLength(0); i++)
        {
            CreateEdge($"道{i}", rawEdgeData[i, 0], rawEdgeData[i, 1], rawEdgeData[i, 2]);
        }

        Debug.Log("<color=cyan><b>=== 【初期状態】ルート計算 ===</b></color>");
        PrintBestPath(0, 8); // A(0) から I(8) へのルート
    }

    private void CreateNode(string name)
    {
        Node node = new Node { id = nodes.Count, name = name };
        nodes.Add(node);
    }

    private void CreateEdge(string name, int from, int to, int score)
    {
        Edge edge = new Edge { id = edges.Count, name = name, fromNode = from, toNode = to, score = score };
        edges.Add(edge);
        nodes[from].edgeIds.Add(edge.id);
    }

    // Unityのインスペクターから右クリックで実行できる関数（チェンジ）
    [ContextMenu("指定した2つの道をチェンジ（入れ替え）")]
    public void SwapEdgeScores()
    {
        if (swapEdgeId1 < 0 || swapEdgeId1 >= edges.Count || swapEdgeId2 < 0 || swapEdgeId2 >= edges.Count)
        {
            Debug.LogError("【エラー】存在しない道IDが指定されています。");
            return;
        }

        // スコアの入れ替え
        int temp = edges[swapEdgeId1].score;
        edges[swapEdgeId1].score = edges[swapEdgeId2].score;
        edges[swapEdgeId2].score = temp;

        Debug.Log($"<color=yellow><b>★【チェンジ】{edges[swapEdgeId1].name} と {edges[swapEdgeId2].name} の数値を入れ替えました！</b></color>");
        
        // 再計算
        PrintBestPath(0, 8);
    }

    // 動的計画法（DP）で最高ルートを計算してConsoleに表示する
    private void PrintBestPath(int start, int goal)
    {
        int[] memo = new int[nodes.Count];
        int[] nextChoice = new int[nodes.Count];
        for (int i = 0; i < memo.Length; i++)
        {
            memo[i] = int.MinValue;
            nextChoice[i] = -1;
        }

        int score = DpRecursive(start, goal, memo, nextChoice);

        if (score == int.MinValue)
        {
            Debug.LogWarning("ゴールに到達できるルートがありません。");
            return;
        }

        string resultPath = $"[{nodes[start].name}]";
        int current = start;
        while (current != goal && nextChoice[current] != -1)
        {
            int edgeId = nextChoice[current];
            Edge edge = edges[edgeId];
            resultPath += $" ─({edge.score})─> [{nodes[edge.toNode].name}]";
            current = edge.toNode;
        }

        Debug.Log($"<b>🤖 エージェントの最大スコアルート結果</b>\n最高スコア: <color=green>{score} 点</color>\nルート: {resultPath}");
    }

    private int DpRecursive(int current, int goal, int[] memo, int[] nextChoice)
    {
        if (current == goal) return 0;
        if (memo[current] != int.MinValue) return memo[current];

        int maxScore = int.MinValue;
        int bestEdge = -1;
        Node node = nodes[current];

        foreach (int edgeId in node.edgeIds)
        {
            Edge edge = edges[edgeId];
            int nextScore = DpRecursive(edge.toNode, goal, memo, nextChoice);

            if (nextScore != int.MinValue)
            {
                int total = edge.score + nextScore;
                if (total > maxScore)
                {
                    maxScore = total;
                    bestEdge = edgeId;
                }
            }
        }

        memo[current] = maxScore;
        nextChoice[current] = bestEdge;
        return maxScore;
    }
}