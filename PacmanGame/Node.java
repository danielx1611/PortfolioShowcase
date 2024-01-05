//---------------- Node class ----------------
   /**
   * Node of the grid, which stores a reference to its
   * element and to its 4 neighbors.
   */
public class Node<E> {

   /** The element stored at this node */
   private E element;               // reference to the element stored at this node

   /** A reference to the preceding node in the list */
   private Node<E> prev;            // reference to the previous node in the list

   /** A reference to the subsequent node in the list */
   private Node<E> next;            // reference to the subsequent node in the list

   private Node<E> above;

   private Node<E> below;

   private boolean visited;

   private int posX;

   private int posY;

   /**
    * Creates a node with the given element and next node.
   *
   * @param e  the element to be stored
   * @param p  reference to a node that should precede the new node
   * @param n  reference to a node that should follow the new node
   * @param a  reference to a node above the node
   * @param b  reference to a node below the node
   */
   public Node(E e, Node<E> p, Node<E> n, Node<E> a, Node<E> b, int x, int y) {
      element = e;
      prev = p;
      next = n;
      above = a;
      below = b;
      visited = false;
      posX = x;
      posY = y;
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
   public Node<E> getPrev() { return prev; }

   /**
    * Returns the node that follows this one (or null if no such node).
   * @return the following node
   */
   public Node<E> getNext() { return next; }

   public Node<E> above() { return above; }

   public Node<E> below() { return below; }

   public boolean isVisited() { return visited; }

   public int getPosX() { return posX; }

   public int getPosY() { return posY; }

   // Update methods
   /**
    * Sets the node's previous reference to point to Node n.
   * @param p    the node that should precede this one
   */
   public void setPrev(Node<E> p) { prev = p; }

   /**
    * Sets the node's next reference to point to Node n.
   * @param n    the node that should follow this one
   */
   public void setNext(Node<E> n) { next = n; }

   public void setAbove(Node<E> a) { above = a; }

   public void setBelow(Node<E> b) { below = b; }

   public void setElement(E e) { element = e; }

   public void setVisited(boolean isVisited) { visited = isVisited; }

   public void setPosX(int x) { posX = x; }

   public void setPosY(int y) { posY = y; }

} //----------- end of Node class -----------
