#include <stdio.h>
#include <string.h>
#include <limits.h>

#define MAX_NODES 100
#define MAX_EDGES 100
#define MAX_EDGES_PER_NODE 10

// -------------------------
// 1. 構造体の定義（クラスの代わり）
// -------------------------

// 道を表す構造体
typedef struct {
    int id;           // 道のID（配列のインデックス）
    char name[32];    // 道の名前（表示用）
    int from_node;    // 出発マスのID
    int to_node;      // 到着マスのID
    int score;        // 道の評価値
} Edge;

// マスを表す構造体
typedef struct {
    int id;           // マスのID
    char name[32];    // マスの名前
    int edge_count;   // このマスから出る道の数
    int edge_ids[MAX_EDGES_PER_NODE]; // 出発する道のIDリスト
} Node;

// マップ全体を管理する構造体
typedef struct {
    Node nodes[MAX_NODES];
    int node_count;
    Edge edges[MAX_EDGES];
    int edge_count;
} GameMap;


// -------------------------
// 2. マップ構築用の関数群
// -------------------------

// マップの初期化
void init_map(GameMap* map) {
    map->node_count = 0;
    map->edge_count = 0;
}

// マスを作成してIDを返す
int create_node(GameMap* map, const char* name) {
    int id = map->node_count++;
    map->nodes[id].id = id;
    strcpy(map->nodes[id].name, name);
    map->nodes[id].edge_count = 0;
    return id;
}

// 道を作成してIDを返す
int create_edge(GameMap* map, const char* name, int from, int to, int score) {
    int id = map->edge_count++;
    map->edges[id].id = id;
    strcpy(map->edges[id].name, name);
    map->edges[id].from_node = from;
    map->edges[id].to_node = to;
    map->edges[id].score = score;
    
    // 出発マスにこの道を登録
    Node* from_n = &map->nodes[from];
    from_n->edge_ids[from_n->edge_count++] = id;
    return id;
}

// 【プレイヤー用】2つの道の評価値を入れ替える
void swap_edge_scores(GameMap* map, int edge1_id, int edge2_id) {
    int temp = map->edges[edge1_id].score;
    map->edges[edge1_id].score = map->edges[edge2_id].score;
    map->edges[edge2_id].score = temp;
    printf("【チェンジ】%s と %s の数値を入れ替えました！\n\n", 
           map->edges[edge1_id].name, map->edges[edge2_id].name);
}


// -------------------------
// 3. 動的計画法（DP）の心臓部
// -------------------------

// 再帰的にDPを計算する内部関数
int dp_recursive(GameMap* map, int current, int goal, int* memo, int* next_choice) {
    // ゴールに到達した場合はスコア0を追加して終了
    if (current == goal) return 0;
    
    // 既に計算済みのマスならメモを返す（計算量の削減）
    if (memo[current] != INT_MIN) return memo[current];

    int max_score = INT_MIN;
    int best_edge = -1;

    Node* node = &map->nodes[current];
    
    // このマスから進める全ての道を試す
    for (int i = 0; i < node->edge_count; i++) {
        int edge_id = node->edge_ids[i];
        Edge* edge = &map->edges[edge_id];
        
        // 移動先のマス以降の最大スコアを取得
        int next_score = dp_recursive(map, edge->to_node, goal, memo, next_choice);
        
        // ゴールに辿り着けない行き止まりルートは除外
        if (next_score != INT_MIN) {
            int total = edge->score + next_score;
            if (total > max_score) {
                max_score = total;
                best_edge = edge_id;
            }
        }
    }

    // 結果をメモに記録
    memo[current] = max_score;
    next_choice[current] = best_edge;
    return max_score;
}

// 最大スコアを計算し、ルートを表示する関数
void print_best_path(GameMap* map, int start, int goal) {
    int memo[MAX_NODES];
    int next_choice[MAX_NODES];
    
    // メモ配列を「未計算（INT_MIN）」で初期化
    for (int i = 0; i < MAX_NODES; i++) {
        memo[i] = INT_MIN;
        next_choice[i] = -1;
    }

    // DP実行
    int score = dp_recursive(map, start, goal, memo, next_choice);

    if (score == INT_MIN) {
        printf("ゴールに到達できるルートがありません。\n");
        return;
    }

    printf("最高スコア: %d 点\n", score);
    printf("ルート: ");
    
    // 記録した選択を辿ってルートを表示
    int current = start;
    while (current != goal && next_choice[current] != -1) {
        int edge_id = next_choice[current];
        Edge* edge = &map->edges[edge_id];
        printf("[%s]", edge->name);
        
        current = edge->to_node;
        if (current != goal) printf(" -> ");
    }
    printf("\n\n");
}


// -------------------------
// 4. メイン関数（シミュレーション）
// -------------------------
int main() {
    GameMap map;
    init_map(&map);

    // ① マスの作成
    int start = create_node(&map, "スタート");
    int top   = create_node(&map, "上のマス");
    int btm   = create_node(&map, "下のマス");
    int goal  = create_node(&map, "ゴール");

    // ② 道の作成（上のルートはマイナス、下のルートはプラス）
    int edge_topA = create_edge(&map, "上の道A", start, top, -5);
    int edge_topB = create_edge(&map, "上の道B", top, goal, -10);
    
    int edge_btmA = create_edge(&map, "下の道A", start, btm, 3);
    int edge_btmB = create_edge(&map, "下の道B", btm, goal, 4);

    // ③ 初期状態の計算
    printf("=== 初期状態 ===\n");
    print_best_path(&map, start, goal);

    // ④ プレイヤーによるチェンジ発動！
    // 「上の道B（-10）」と「下の道B（+4）」を入れ替える
    swap_edge_scores(&map, edge_topB, edge_btmB);

    // ⑤ チェンジ後の再計算
    printf("=== チェンジ後 ===\n");
    print_best_path(&map, start, goal);

    return 0;
}