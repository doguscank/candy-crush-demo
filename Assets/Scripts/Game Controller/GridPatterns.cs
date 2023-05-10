using System.Collections;
using System.Collections.Generic;

public class GridPatterns : IEnumerable<GridPattern>
{
    private List<GridPattern> mPatterns;

    public GridPatterns(List<GridPattern> patterns)
    {
        mPatterns = patterns;
    }

    public GridPatterns()
    {
        mPatterns = new List<GridPattern>();

        // Vertical 5-matches
        mPatterns.Add(
            new GridPattern(
                Powerups.PowerupType.ColorRemover,
                new List<(int, int)> {
                    (1, 0),
                    (2, 0),
                    (3, 0),
                    (4, 0)
                }
            )
        );

        // Horizontal 5-matches
        mPatterns.Add(
            new GridPattern(
                Powerups.PowerupType.ColorRemover,
                new List<(int, int)> {
                    (0, 1),
                    (0, 2),
                    (0, 3),
                    (0, 4)
                }
            )
        );

        // T-type patterns
        mPatterns.Add(
            new GridPattern(
                Powerups.PowerupType.Bomb,
                new List<(int, int)> {
                    (0, 1),
                    (0, 2),
                    (-1, 1),
                    (-2, 1)
                }
            )
        );
        mPatterns.Add(
            new GridPattern(
                Powerups.PowerupType.Bomb,
                new List<(int, int)> {
                    (1, 0),
                    (2, 0),
                    (1, 1),
                    (1, 2)
                }
            )
        );
        mPatterns.Add(
            new GridPattern(
                Powerups.PowerupType.Bomb,
                new List<(int, int)> {
                    (0, 1),
                    (0, 2),
                    (-1, 2),
                    (1, 2)
                }
            )
        );
        mPatterns.Add(
            new GridPattern(
                Powerups.PowerupType.Bomb,
                new List<(int, int)> {
                    (0, 1),
                    (0, 2),
                    (1, 1),
                    (2, 1)
                }
            )
        );

        // L-type patterns
        mPatterns.Add(
            new GridPattern(
                Powerups.PowerupType.Bomb,
                new List<(int, int)> {
                    (1, 0),
                    (2, 0),
                    (2, 1),
                    (2, 2)
                }
            )
        );
        mPatterns.Add(
            new GridPattern(
                Powerups.PowerupType.Bomb,
                new List<(int, int)> {
                    (1, 0),
                    (2, 0),
                    (2, -1),
                    (2, -2)
                }
            )
        );
        mPatterns.Add(
            new GridPattern(
                Powerups.PowerupType.Bomb,
                new List<(int, int)> {
                    (1, 0),
                    (2, 0),
                    (0, 1),
                    (0, 2)
                }
            )
        );
        mPatterns.Add(
            new GridPattern(
                Powerups.PowerupType.Bomb,
                new List<(int, int)> {
                    (0, 1),
                    (0, 2),
                    (2, 1),
                    (2, 2)
                }
            )
        );

        // Vertical 4-matches
        mPatterns.Add(
            new GridPattern(
                Powerups.PowerupType.ColumnRemover,
                new List<(int, int)> {
                    (1, 0),
                    (2, 0),
                    (3, 0)
                }
            )
        );

        // Horizontal 4-matches
        mPatterns.Add(
            new GridPattern(
                Powerups.PowerupType.RowRemover,
                new List<(int, int)> {
                    (0, 1),
                    (0, 2),
                    (0, 3)
                }
            )
        );

        // Vertical 3-matches
        mPatterns.Add(
            new GridPattern(
                Powerups.PowerupType.NoPowerup,
                new List<(int, int)> {
                    (1, 0),
                    (2, 0)
                }
            )
        );

        // Horizontal 3-matches
        mPatterns.Add(
            new GridPattern(
                Powerups.PowerupType.NoPowerup,
                new List<(int, int)> {
                    (0, 1),
                    (0, 2)
                }
            )
        );
    }

    public IEnumerator<GridPattern> GetEnumerator()
    {
        foreach (var item in mPatterns)
        {
            yield return item;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}