namespace Backuper.Abstractions.TestingHelpers;

public class SearchPattern {

    public SearchPattern(string searchPattern) {
        _searchPattern = searchPattern;
    }

    private readonly string _searchPattern;

    public bool Match(string path) => Match(path, _searchPattern);

    public const char Wildcard = '*';
    public const char OneCharWildcard = '?';

    public static bool Match(string path, string searchPattern) {
        searchPattern = searchPattern.Trim();

        if(searchPattern == "." || searchPattern.All(x => x == Wildcard)) {
            return true;
        }

        if(!searchPattern.Contains(Wildcard)) {
            return MatchKey(path, searchPattern);
        }

        var keys = searchPattern.Split(Wildcard, StringSplitOptions.RemoveEmptyEntries);
        int currKey = 0;
        int currIndex = 0;

        //make sure the first key starts the path if there's no wildcard
        if(searchPattern[0] != Wildcard) {
            var keyLen = keys[0].Length;

            if(path.Length <= keyLen || !MatchKey(path[..keyLen], keys[0])) {
                return false;
            }

            currKey++;
            currIndex += keyLen;

            if(keys.Length == 1) {

                //if it doesn't end with a wild card, the match should end at the end of the path
                if(searchPattern[^1] != '*') {
                    return currIndex == path.Length;
                }

                return true;
            }

        }

        while(currIndex < path.Length) {

            //if it matches the first key, check all the remaining ones
            if(path[currIndex] == keys[currKey][0] || keys[currKey][0] == OneCharWildcard) {

                var keyLen = keys[currKey].Length;

                //the remaining string is too short
                if(currIndex + keyLen > path.Length) {
                    return false;
                }

                if(MatchKey(path.Substring(currIndex, keyLen), keys[currKey])) {

                    currKey++;
                    currIndex += keyLen;
                    if(currKey >= keys.Length) {

                        //if it doesn't end with a wild card, the match should end at the end of the path
                        if(searchPattern[^1] != '*') {
                            return currIndex == path.Length;
                        }

                        return true;
                    }

                    //instead of adding multiple "else" conditions on whether to increment this loop's currIndex,
                    //I've decided to decrement it here 
                    currIndex--;

                }

            }

            currIndex++;
        }

        return false;
    }

    private static bool MatchKey(string s, string key) {

        if(s.Length != key.Length) {
            return false;
        }

        return Enumerable
            .Range(0, s.Length)
            .All(i => s[i] == key[i] || key[i] == OneCharWildcard);
    }

}