import { useState, useEffect } from 'react';
import { Navbar } from './components/Navbar';
import { VideoGrid } from './components/VideoGrid';
import { WatchPage } from './components/WatchPage';
import { UploadModal } from './components/UploadModal';
import { initialVideos, initialComments } from './mockData';
import type { Video, Comment } from './types';

function App() {
  const [videosList, setVideosList] = useState<Video[]>(initialVideos);
  const [commentsMap, setCommentsMap] = useState<Record<string, Comment[]>>(initialComments);
  const [view, setView] = useState<'home' | 'watch'>('home');
  const [activeVideoId, setActiveVideoId] = useState<string | null>(null);
  const [searchQuery, setSearchQuery] = useState('');
  const [isUploadOpen, setIsUploadOpen] = useState(false);
  const [theme, setTheme] = useState<'dark' | 'light'>('dark');

  // Sync theme with DOM attribute
  useEffect(() => {
    document.documentElement.setAttribute('data-theme', theme);
  }, [theme]);

  const toggleTheme = () => {
    setTheme((prev) => (prev === 'dark' ? 'light' : 'dark'));
  };

  const handleVideoSelect = (video: Video) => {
    // Increment views mockup
    setVideosList((prevList) =>
      prevList.map((v) => {
        if (v.id === video.id) {
          const currentViews = v.viewsCount + 1;
          const formattedViews = currentViews >= 1000000 
            ? `${(currentViews / 1000000).toFixed(1)}M views` 
            : currentViews >= 1000 
              ? `${(currentViews / 1000).toFixed(0)}K views` 
              : `${currentViews} views`;
          return {
            ...v,
            viewsCount: currentViews,
            views: formattedViews,
          };
        }
        return v;
      })
    );
    setActiveVideoId(video.id);
    setView('watch');
    window.scrollTo({ top: 0, behavior: 'smooth' });
  };

  const handleLikeVideo = (videoId: string, isDislike = false) => {
    setVideosList((prevList) =>
      prevList.map((v) => {
        if (v.id !== videoId) return v;

        let nextLikes = v.likes;
        let nextLikedByMe = v.likedByMe;
        let nextDislikedByMe = v.dislikedByMe;

        if (isDislike) {
          if (nextDislikedByMe) {
            nextDislikedByMe = false;
          } else {
            nextDislikedByMe = true;
            if (nextLikedByMe) {
              nextLikedByMe = false;
              nextLikes = Math.max(0, nextLikes - 1);
            }
          }
        } else {
          if (nextLikedByMe) {
            nextLikedByMe = false;
            nextLikes = Math.max(0, nextLikes - 1);
          } else {
            nextLikedByMe = true;
            nextLikes += 1;
            if (nextDislikedByMe) {
              nextDislikedByMe = false;
            }
          }
        }

        return {
          ...v,
          likes: nextLikes,
          likedByMe: nextLikedByMe,
          dislikedByMe: nextDislikedByMe,
        };
      })
    );
  };

  const handleAddComment = (videoId: string, text: string, replyToId?: string) => {
    const newCommentId = `comment-${Date.now()}`;
    const commentAuthor = {
      userName: 'AuraViewer',
      userAvatar: 'https://images.unsplash.com/photo-1534528741775-53994a69daeb?w=150&auto=format&fit=crop&q=80',
    };

    setCommentsMap((prevMap) => {
      const activeComments = prevMap[videoId] ? [...prevMap[videoId]] : [];

      if (replyToId) {
        // Find parent comment
        const updatedComments = activeComments.map((comment) => {
          if (comment.id === replyToId) {
            const currentReplies = comment.replies ? [...comment.replies] : [];
            const newReply: Comment = {
              id: newCommentId,
              userName: commentAuthor.userName,
              userAvatar: commentAuthor.userAvatar,
              text: text,
              timestamp: 'Just now',
              likes: 0,
              likedByMe: false,
            };
            return {
              ...comment,
              replies: [...currentReplies, newReply],
            };
          }
          return comment;
        });
        return {
          ...prevMap,
          [videoId]: updatedComments,
        };
      } else {
        // Top level comment
        const newComment: Comment = {
          id: newCommentId,
          userName: commentAuthor.userName,
          userAvatar: commentAuthor.userAvatar,
          text: text,
          timestamp: 'Just now',
          likes: 0,
          likedByMe: false,
          replies: [],
        };
        return {
          ...prevMap,
          [videoId]: [newComment, ...activeComments],
        };
      }
    });
  };

  const handleLikeComment = (commentId: string) => {
    if (!activeVideoId) return;

    setCommentsMap((prevMap) => {
      const activeComments = prevMap[activeVideoId] ? [...prevMap[activeVideoId]] : [];

      const updatedComments = activeComments.map((comment) => {
        // Check top-level comment
        if (comment.id === commentId) {
          const liked = !comment.likedByMe;
          return {
            ...comment,
            likedByMe: liked,
            likes: liked ? comment.likes + 1 : Math.max(0, comment.likes - 1),
          };
        }

        // Check replies
        if (comment.replies && comment.replies.length > 0) {
          const updatedReplies = comment.replies.map((reply) => {
            if (reply.id === commentId) {
              const liked = !reply.likedByMe;
              return {
                ...reply,
                likedByMe: liked,
                likes: liked ? reply.likes + 1 : Math.max(0, reply.likes - 1),
              };
            }
            return reply;
          });
          return {
            ...comment,
            replies: updatedReplies,
          };
        }

        return comment;
      });

      return {
        ...prevMap,
        [activeVideoId]: updatedComments,
      };
    });
  };

  const handleUploadSuccess = (newVideo: Video) => {
    setVideosList((prevList) => [newVideo, ...prevList]);
    setCommentsMap((prevMap) => ({
      ...prevMap,
      [newVideo.id]: [],
    }));
    setActiveVideoId(newVideo.id);
    setView('watch');
    setIsUploadOpen(false);
    window.scrollTo({ top: 0, behavior: 'smooth' });
  };

  const activeVideo = videosList.find((v) => v.id === activeVideoId);

  return (
    <>
      <Navbar
        searchQuery={searchQuery}
        setSearchQuery={setSearchQuery}
        onUploadClick={() => setIsUploadOpen(true)}
        theme={theme}
        toggleTheme={toggleTheme}
        onLogoClick={() => {
          setView('home');
          setActiveVideoId(null);
        }}
      />

      <main style={{ flexGrow: 1, display: 'flex', flexDirection: 'column' }}>
        {view === 'home' || !activeVideo ? (
          <VideoGrid
            videos={videosList}
            searchQuery={searchQuery}
            onVideoSelect={handleVideoSelect}
          />
        ) : (
          <WatchPage
            video={activeVideo}
            allVideos={videosList}
            comments={commentsMap[activeVideo.id] || []}
            onVideoSelect={handleVideoSelect}
            onLikeVideo={handleLikeVideo}
            onAddComment={handleAddComment}
            onLikeComment={handleLikeComment}
          />
        )}
      </main>

      {/* Footer */}
      <footer style={{
        padding: '24px',
        textAlign: 'center',
        borderTop: '1px solid var(--border-color)',
        fontSize: '0.85rem',
        color: 'var(--text-muted)',
        marginTop: 'auto',
      }}>
        <p>© 2026 VibeStream Inc. Built with React & TypeScript.</p>
      </footer>

      {/* Upload Modal Overlay */}
      {isUploadOpen && (
        <UploadModal
          onClose={() => setIsUploadOpen(false)}
          onUploadSuccess={handleUploadSuccess}
        />
      )}
    </>
  );
}

export default App;
