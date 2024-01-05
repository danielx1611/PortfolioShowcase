//---------------- Dijkstra Node class ----------------
   /**
   * Node of the grid, which stores a reference to its
   * element and to its 4 neighbors.
   */
  public class DijkstraNode<E> {

   /** The element stored at this node */
   private E element;               // reference to the element stored at this node

   /** A reference to the preceding node in the list */
   private DijkstraNode<E> prev;            // reference to the previous node in the list

   /** A reference to the subsequent node in the list */
   private DijkstraNode<E> next;            // reference to the subsequent node in the list

   private DijkstraNode<E> above;

   private DijkstraNode<E> below;

   private DijkstraNode<E> parent;

   private boolean visited;

   private int posX;

   private int posY;

   private int dValue;
   private int weightNext = 0;
   private int weightPrev = 0;
   private int weightAbove = 0;
   private int weightBelow = 0;

   /**
    * Creates a node with the given element and next node.
   *
   * @param e  the element to be stored
   * @param p  reference to a node that should precede the new node
   * @param n  reference to a node that should follow the new node
   * @param a  reference to a node above the node
   * @param b  reference to a node below the node
   */
   public DijkstraNode(E e, DijkstraNode<E> p, DijkstraNode<E> n, DijkstraNode<E> a, DijkstraNode<E> b, int x, int y, int d) {
      element = e;
      prev = p;
      next = n;
      above = a;
      below = b;
      visited = false;
      posX = x;
      posY = y;
      dValue = d;
   }

   // public accessor methods
   /**
    * Returns the element stored at the node.
   * @return the element stored at the node
   */
   public E getElement() { return element; }

   /**
    * Returns the node that precedes this one (or null if no such node).
   * @return the preceding node
   */
   public DijkstraNode<E> getPrev() { return prev; }

   /**
    * Returns the node that follows this one (or null if no such node).
   * @return the following node
   */
   public DijkstraNode<E> getNext() { return next; }

   public DijkstraNode<E> above() { return above; }

   public DijkstraNode<E> below() { return below; }

   public DijkstraNode<E> parent() { return parent; }

   public boolean isVisited() { return visited; }

   public int getPosX() { return posX; }

   public int getPosY() { return posY; }

   public int getWeightNext() { return weightNext; }

   public int getWeightPrev() { return weightPrev; }

   public int getWeightAbove() { return weightAbove; }

   public int getWeightBelow() { return weightBelow; }

   public int getDValue() { return dValue; }

   // Update methods
   /**
    * Sets the node's previous reference to point to Node n.
   * @param p    the node that should precede this one
   */
   public void setPrev(DijkstraNode<E> p) { prev = p; }

   /**
    * Sets the node's next reference to point to Node n.
   * @param n    the node that should follow this one
   */
   public void setNext(DijkstraNode<E> n) { next = n; }

   public void setAbove(DijkstraNode<E> a) { above = a; }

   public void setBelow(DijkstraNode<E> b) { below = b; }

   public void setParent(DijkstraNode<E> p) { parent = p; }

   public void setElement(E e) { element = e; }

   public void setVisited(boolean isVisited) { visited = isVisited; }

   public void setPosX(int x) { posX = x; }

   public void setPosY(int y) { posY = y; }

   public void setWeightNext(int weight) { weightNext = weight; }

   public void setWeightPrev(int weight) { weightPrev = weight; }

   public void setWeightAbove(int weight) { weightAbove = weight; }

   public void setWeightBelow(int weight) { weightBelow = weight; }

   public void setDValue(int value) { dValue = value; }

} //----------- end of Dijkstra Node class -----------