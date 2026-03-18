export class Connect4 {

  board: number[][]; // 6 rows x 7 columns
  currentPlayer: number; // 1 or 2
  finished: boolean; // true if the game is over

  constructor() {
    // Initialize the board and set the current player
    // this.board = Array.from({ length: 6 }, () => Array(7).fill(0));
    this.board = [];
    for (let i = 0; i < 6; i++) {
      this.board[i] = [];
      for (let j = 0; j < 7; j++) {
        this.board[i][j] = 0;
      }
    }
    this.currentPlayer = 1;
    this.finished = false;
  }

  play(col: number): string {
    // If the game is finished, return a message
    if (this.finished) {
      return "Game has finished!";
    }

    // It just means I haven't found a spot yet
    let row = -1;

    // Find the lowest empty row in the specified column
    for (let i = 5; i >= 0; i--) {
      if (this.board[i][col] === 0) {
        row = i;
        break;
      }
    }

    // If the column is full, return a message
    if (row === -1) {
      return "Column full!";
    }

    // Place the current player's piece in the board
    this.board[row][col] = this.currentPlayer;

    // Save the player before changing him
    const player = this.currentPlayer;
    this.board[row][col] = player;


    // Check vertical victory
    let count = 1;  // Include the piece placed

    // UP victory check
    let r = row - 1;
    // Count consecutive pieces upwards in the same column. Starts from the row above the last placed piece
    while (r >= 0 && this.board[r][col] === player) {
      count++;
      r--;
    }

    // Down victory check
    r = row + 1;
    while (r < 6 && this.board[r][col] === player) {
      count++;
      r++;
    }

    if (count >= 4) {
      this.finished = true;
      return `Player ${player} wins!`;
    }

    // Check horizontal victory
    count = 1;

    // Left victory check
    let c = col - 1;
    while (c >= 0 && this.board[row][c] === player) {
      count++;
      c--;
    }

    // Right victory check
    c = col + 1;
    while (c < 7 && this.board[row][c] === player) {
      count++;
      c++;
    }

    if (count >= 4) {
      this.finished = true;
      return `Player ${player} wins!`;
    }

    // Normal message if no one wins
    const message = `Player ${player} has a turn`;

    // Switch to the other player
    this.currentPlayer = this.currentPlayer === 1 ? 2 : 1;

    return message;

  }
}
