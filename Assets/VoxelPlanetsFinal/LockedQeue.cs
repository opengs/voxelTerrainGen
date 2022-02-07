using System.Collections.Generic;
namespace System.Collections.Generic{
	public class LockedQeue<T>{
		private Queue<T> q = new Queue<T>();
		public void Enqueue(T value){
			lock(q){
				q.Enqueue(value);
			}
		}
		public T Dequeue(){
			lock(q){
				return q.Dequeue();
			}
		}
		public int Count{
			get{
				lock(q){
					return q.Count;
				}
			}
		}
	}
}
