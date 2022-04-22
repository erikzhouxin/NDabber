using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace System.Data.Dubber
{
    /// <summary>
    /// Only accept files that have the given extension, or exclude files of a given extension.
    /// </summary>
    public class FtpFileExtensionRule : FtpRule
    {

        /// <summary>
        /// If true, only files of the given extension are uploaded or downloaded. If false, files of the given extension are excluded.
        /// </summary>
        public bool Whitelist;

        /// <summary>
        /// The extensions to match
        /// </summary>
        public IList<string> Exts;

        /// <summary>
        /// Only accept files that have the given extension, or exclude files of a given extension.
        /// </summary>
        /// <param name="whitelist">If true, only files of the given extension are uploaded or downloaded. If false, files of the given extension are excluded.</param>
        /// <param name="exts">The extensions to match</param>
        public FtpFileExtensionRule(bool whitelist, IList<string> exts)
        {
            this.Whitelist = whitelist;
            this.Exts = exts;
        }

        /// <summary>
        /// Checks if the files has the given extension, or exclude files of the given extension.
        /// </summary>
        public override bool IsAllowed(FtpListItem item)
        {
            if (item.Type == FtpFileSystemObjectType.File)
            {
                var ext = Path.GetExtension(item.Name).Replace(".", "").ToLower();
                if (Whitelist)
                {

                    // whitelist
                    if (ext.IsBlank())
                    {
                        return false;
                    }
                    else
                    {
                        return Exts.Contains(ext);
                    }
                }
                else
                {

                    // blacklist
                    if (ext.IsBlank())
                    {
                        return true;
                    }
                    else
                    {
                        return !Exts.Contains(ext);
                    }
                }
            }
            else
            {
                return true;
            }
        }

    }
    /// <summary>
    /// Only accept files whose names match the given regular expression(s), or exclude files that match.
    /// </summary>
    public class FtpFileNameRegexRule : FtpRule
    {

        /// <summary>
        /// If true, only items where one of the supplied regex pattern matches are uploaded or downloaded.
        /// If false, items where one of the supplied regex pattern matches are excluded.
        /// </summary>
        public bool Whitelist;

        /// <summary>
        /// The files names to match
        /// </summary>
        public List<string> RegexPatterns;

        /// <summary>
        /// Only accept items that match one of the supplied regex patterns.
        /// </summary>
        /// <param name="whitelist">If true, only items where one of the supplied regex pattern matches are uploaded or downloaded. If false, items where one of the supplied regex pattern matches are excluded.</param>
        /// <param name="regexPatterns">The list of regex patterns to match. Only valid patterns are accepted and stored. If none of the patterns are valid, this rule is disabled and passes all objects.</param>
        public FtpFileNameRegexRule(bool whitelist, IList<string> regexPatterns)
        {
            this.Whitelist = whitelist;
            this.RegexPatterns = regexPatterns.Where(x => x.IsValidRegEx()).ToList();
        }

        /// <summary>
        /// Checks if the FtpListItem Name does match any RegexPattern
        /// </summary>
        public override bool IsAllowed(FtpListItem item)
        {

            // if no valid regex patterns, accept all objects
            if (RegexPatterns.Count == 0)
            {
                return true;
            }

            // only check files
            if (item.Type == FtpFileSystemObjectType.File)
            {
                var fileName = item.Name;

                if (Whitelist)
                {
                    return RegexPatterns.Any(x => Regex.IsMatch(fileName, x));
                }
                else
                {
                    return !RegexPatterns.Any(x => Regex.IsMatch(fileName, x));
                }
            }
            else
            {
                return true;
            }
        }

    }
    /// <summary>
    /// Only accept files that have the given name, or exclude files of a given name.
    /// </summary>
    public class FtpFileNameRule : FtpRule
    {

        /// <summary>
        /// If true, only files of the given name are uploaded or downloaded. If false, files of the given name are excluded.
        /// </summary>
        public bool Whitelist;

        /// <summary>
        /// The files names to match
        /// </summary>
        public IList<string> Names;

        /// <summary>
        /// Only accept files that have the given name, or exclude files of a given name.
        /// </summary>
        /// <param name="whitelist">If true, only files of the given name are downloaded. If false, files of the given name are excluded.</param>
        /// <param name="names">The files names to match</param>
        public FtpFileNameRule(bool whitelist, IList<string> names)
        {
            this.Whitelist = whitelist;
            this.Names = names;
        }

        /// <summary>
        /// Checks if the files has the given name, or exclude files of the given name.
        /// </summary>
        public override bool IsAllowed(FtpListItem item)
        {
            if (item.Type == FtpFileSystemObjectType.File)
            {
                var fileName = item.Name;
                if (Whitelist)
                {
                    return Names.Contains(fileName);
                }
                else
                {
                    return !Names.Contains(fileName);
                }
            }
            else
            {
                return true;
            }
        }

    }
    /// <summary>
    /// Only accept folders whose names match the given regular expression(s), or exclude folders that match.
    /// </summary>
    public class FtpFolderRegexRule : FtpRule
    {

        /// <summary>
        /// If true, only folders where one of the supplied regex pattern matches are uploaded or downloaded.
        /// If false, folders where one of the supplied regex pattern matches are excluded.
        /// </summary>
        public bool Whitelist;

        /// <summary>
        /// The files names to match
        /// </summary>
        public List<string> RegexPatterns;

        /// <summary>
        /// Which path segment to start checking from
        /// </summary>
        public int StartSegment;

        /// <summary>
        /// Only accept items that one of the supplied regex pattern.
        /// </summary>
        /// <param name="whitelist">If true, only folders where one of the supplied regex pattern matches are uploaded or downloaded. If false, folders where one of the supplied regex pattern matches are excluded.</param>
        /// <param name="regexPatterns">The list of regex patterns to match. Only valid patterns are accepted and stored. If none of the patterns are valid, this rule is disabled and passes all objects.</param>
        /// <param name="startSegment">Which path segment to start checking from. 0 checks root folder onwards. 1 skips root folder.</param>
        public FtpFolderRegexRule(bool whitelist, IList<string> regexPatterns, int startSegment = 0)
        {
            this.Whitelist = whitelist;
            this.RegexPatterns = regexPatterns.Where(x => x.IsValidRegEx()).ToList();
            this.StartSegment = startSegment;
        }

        /// <summary>
        /// Checks if the FtpListItem Name does match any RegexPattern
        /// </summary>
        public override bool IsAllowed(FtpListItem item)
        {

            // if no valid regex patterns, accept all objects
            if (RegexPatterns.Count == 0)
            {
                return true;
            }

            // get the folder name of this item
            string[] dirNameParts = null;
            if (item.Type == FtpFileSystemObjectType.File)
            {
                dirNameParts = item.FullName.GetFtpDirectoryName().GetPathSegments();
            }
            else if (item.Type == FtpFileSystemObjectType.Directory)
            {
                dirNameParts = item.FullName.GetPathSegments();
            }
            else
            {
                return true;
            }

            // check against whitelist or blacklist
            if (Whitelist)
            {

                // loop thru path segments starting at given index
                for (int d = StartSegment; d < dirNameParts.Length; d++)
                {
                    var dirName = dirNameParts[d];

                    // whitelist
                    foreach (var pattern in RegexPatterns)
                    {
                        if (Regex.IsMatch(dirName.Trim(), pattern))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            else
            {

                // loop thru path segments starting at given index
                for (int d = StartSegment; d < dirNameParts.Length; d++)
                {
                    var dirName = dirNameParts[d];

                    // blacklist
                    foreach (var pattern in RegexPatterns)
                    {
                        if (Regex.IsMatch(dirName.Trim(), pattern))
                        {
                            return false;
                        }
                    }
                }
                return true;
            }

        }

    }
    /// <summary>
    /// Only accept folders that have the given name, or exclude folders of a given name.
    /// </summary>
    public class FtpFolderNameRule : FtpRule
    {

        public static List<string> CommonBlacklistedFolders = new List<string> {
            ".git",
            ".svn",
            ".DS_Store",
            "node_modules",
        };

        /// <summary>
        /// If true, only folders of the given name are uploaded or downloaded.
        /// If false, folders of the given name are excluded.
        /// </summary>
        public bool Whitelist;

        /// <summary>
        /// The folder names to match
        /// </summary>
        public IList<string> Names;

        /// <summary>
        /// Which path segment to start checking from
        /// </summary>
        public int StartSegment;

        /// <summary>
        /// Only accept folders that have the given name, or exclude folders of a given name.
        /// </summary>
        /// <param name="whitelist">If true, only folders of the given name are downloaded. If false, folders of the given name are excluded.</param>
        /// <param name="names">The folder names to match</param>
        /// <param name="startSegment">Which path segment to start checking from. 0 checks root folder onwards. 1 skips root folder.</param>
        public FtpFolderNameRule(bool whitelist, IList<string> names, int startSegment = 0)
        {
            this.Whitelist = whitelist;
            this.Names = names;
            this.StartSegment = startSegment;
        }

        /// <summary>
        /// Checks if the folders has the given name, or exclude folders of the given name.
        /// </summary>
        public override bool IsAllowed(FtpListItem item)
        {

            // get the folder name of this item
            string[] dirNameParts = null;
            if (item.Type == FtpFileSystemObjectType.File)
            {
                dirNameParts = item.FullName.GetFtpDirectoryName().GetPathSegments();
            }
            else if (item.Type == FtpFileSystemObjectType.Directory)
            {
                dirNameParts = item.FullName.GetPathSegments();
            }
            else
            {
                return true;
            }

            // check against whitelist or blacklist
            if (Whitelist)
            {

                // loop thru path segments starting at given index
                for (int d = StartSegment; d < dirNameParts.Length; d++)
                {
                    var dirName = dirNameParts[d];

                    // whitelist
                    if (Names.Contains(dirName.Trim()))
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {

                // loop thru path segments starting at given index
                for (int d = StartSegment; d < dirNameParts.Length; d++)
                {
                    var dirName = dirNameParts[d];

                    // blacklist
                    if (Names.Contains(dirName.Trim()))
                    {
                        return false;
                    }
                }
                return true;
            }
        }

    }
    /// <summary>
    /// Base class used for all FTP Rules. Extend this class to create custom rules.
    /// You only need to provide an implementation for IsAllowed, and add any custom arguments that you require.
    /// </summary>
    public class FtpRule
    {

        public FtpRule()
        {
        }

        /// <summary>
        /// Returns true if the object has passed this rules.
        /// </summary>
        public virtual bool IsAllowed(FtpListItem result)
        {
            return true;
        }

        /// <summary>
        /// Returns true if the object has passed all the rules.
        /// </summary>
        public static bool IsAllAllowed(List<FtpRule> rules, FtpListItem result)
        {
            foreach (var rule in rules)
            {
                if (!rule.IsAllowed(result))
                {
                    return false;
                }
            }
            return true;
        }

    }
    /// <summary>
    /// Only accept files that are of the given size, or within the given range of sizes.
    /// </summary>
    public class FtpSizeRule : FtpRule
    {

        /// <summary>
        /// Which operator to use
        /// </summary>
        public FtpOperator Operator;

        /// <summary>
        /// The first value, required for all operators
        /// </summary>
        public long X;

        /// <summary>
        /// The second value, only required for BetweenRange and OutsideRange operators
        /// </summary>
        public long Y;

        /// <summary>
        /// Only accept files that are of the given size, or within the given range of sizes.
        /// </summary>
        /// <param name="ruleOperator">Which operator to use</param>
        /// <param name="x">The first value, required for all operators</param>
        /// <param name="y">The second value, only required for BetweenRange and OutsideRange operators.</param>
        public FtpSizeRule(FtpOperator ruleOperator, long x, long y = 0)
        {
            this.Operator = ruleOperator;
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Checks if the file is of the given size, or within the given range of sizes.
        /// </summary>
        public override bool IsAllowed(FtpListItem result)
        {
            if (result.Type == FtpFileSystemObjectType.File)
            {
                return Operators.Validate(Operator, result.Size, X, Y);
            }
            else
            {
                return true;
            }
        }

    }
}
