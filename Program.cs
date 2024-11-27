//*****************************************************************************
//** 3243 Shortest Distance After Road Addition Queries I    leetcode        **
//*****************************************************************************

// Min-heap structure
typedef struct {
    int *data;
    int *dist;
    int size;
    int capacity;
} MinHeap;

// Create a new min-heap
MinHeap* createMinHeap(int capacity)
{
    MinHeap* heap = (MinHeap*)malloc(sizeof(MinHeap));
    heap->data = (int*)malloc(capacity * sizeof(int));
    heap->dist = (int*)malloc(capacity * sizeof(int));
    heap->size = 0;
    heap->capacity = capacity;
    return heap;
}

// Free the min-heap
void freeMinHeap(MinHeap* heap)
{
    free(heap->data);
    free(heap->dist);
    free(heap);
}

// Heapify down
void heapifyDown(MinHeap* heap, int idx)
{
    int smallest = idx;
    int left = 2 * idx + 1;
    int right = 2 * idx + 2;

    if (left < heap->size && heap->dist[heap->data[left]] < heap->dist[heap->data[smallest]]) {
        smallest = left;
    }
    if (right < heap->size && heap->dist[heap->data[right]] < heap->dist[heap->data[smallest]]) {
        smallest = right;
    }
    if (smallest != idx) {
        int temp = heap->data[idx];
        heap->data[idx] = heap->data[smallest];
        heap->data[smallest] = temp;
        heapifyDown(heap, smallest);
    }
}

// Heapify up
void heapifyUp(MinHeap* heap, int idx)
{
    int parent = (idx - 1) / 2;
    if (idx && heap->dist[heap->data[idx]] < heap->dist[heap->data[parent]]) {
        int temp = heap->data[idx];
        heap->data[idx] = heap->data[parent];
        heap->data[parent] = temp;
        heapifyUp(heap, parent);
    }
}

// Push to heap
void pushHeap(MinHeap* heap, int node, int dist)
{
    heap->dist[node] = dist;
    heap->data[heap->size] = node;
    heapifyUp(heap, heap->size++);
}

// Pop from heap
int popHeap(MinHeap* heap)
{
    int top = heap->data[0];
    heap->data[0] = heap->data[--heap->size];
    heapifyDown(heap, 0);
    return top;
}

// Check if heap is empty
int isEmpty(MinHeap* heap)
{
    return heap->size == 0;
}

// Function to find the shortest path using Dijkstra's algorithm
int dijkstra(int** adj, int* adjSize, int* dist, int u, int v, int n)
{
    for (int i = 0; i < n; ++i) {
        dist[i] = INT_MAX;
    }
    dist[u] = 0;

    MinHeap* heap = createMinHeap(n);
    pushHeap(heap, u, 0);

    while (!isEmpty(heap)) {
        int curr = popHeap(heap);
        for (int i = 0; i < adjSize[curr]; ++i) {
            int next = adj[curr][2 * i];
            int weight = adj[curr][2 * i + 1];
            if (dist[curr] + weight < dist[next]) {
                dist[next] = dist[curr] + weight;
                pushHeap(heap, next, dist[next]);
            }
        }
    }

    int result = dist[v];
    freeMinHeap(heap);
    return result;
}

// Main function
int* shortestDistanceAfterQueries(int n, int** queries, int queriesSize, int* queriesColSize, int* returnSize)
{
    int** adj = (int**)malloc(n * sizeof(int*));
    int* adjSize = (int*)calloc(n, sizeof(int));
    for (int i = 0; i < n; ++i) {
        adj[i] = (int*)malloc(n * 2 * sizeof(int));
    }

    for (int u = 0; u + 1 < n; ++u) {
        adj[u][2 * adjSize[u]] = u + 1;
        adj[u][2 * adjSize[u] + 1] = 1;
        adjSize[u]++;
    }

    int* dist = (int*)malloc(n * sizeof(int));
    int* result = (int*)malloc(queriesSize * sizeof(int));
    *returnSize = queriesSize;

    for (int i = 0; i < queriesSize; ++i) {
        int u = queries[i][0];
        int v = queries[i][1];

        // Add edge
        adj[u][2 * adjSize[u]] = v;
        adj[u][2 * adjSize[u] + 1] = 1;
        adjSize[u]++;

        result[i] = dijkstra(adj, adjSize, dist, 0, n - 1, n);
    }

    // Free memory
    for (int i = 0; i < n; ++i) {
        free(adj[i]);
    }
    free(adj);
    free(adjSize);
    free(dist);

    return result;
}