export function findOutlier(integers: number[]): number {

  // Filter the even and odd numbers from the input array
  const even = integers.filter(x => x % 2 === 0);
  const odd = integers.filter(x => x % 2 !== 0);

  // Return the single even number if there is only one, otherwise return the single odd number
  return even.length === 1 ? even[0] : odd[0];
}
